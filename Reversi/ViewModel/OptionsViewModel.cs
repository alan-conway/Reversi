using Prism.Commands;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Services.MessageDialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reversi.ViewModel
{
    public class OptionsViewModel : ViewModelBase, IDialogViewModel
    {
        private bool _userStartsNewGames;
        private DialogChoice _dialogChoice;
        private string _selectedAlgorithm;
        private int _selectedLevel;

        public OptionsViewModel(IGameOptions options, IEnumerable<StrategyInfo> availableStrategies)
        {
            _dialogChoice = DialogChoice.Cancel;

            SaveOptionsCommand = new DelegateCommand<IDialogWindow>(ExecuteSaveOptions);
            UndoChangesCommand = new DelegateCommand<IDialogWindow>(ExecuteUndoChanges);

            StartingPlayerChoices = new[] {
                new KeyValuePair<string, bool>("Human", true),
                new KeyValuePair<string, bool>("Computer", false) };
            AlgorithmChoices = new ObservableCollection<string>();
            AlgorithmLevels = Enumerable.Range(1,9);

            UpdateWithStrategies(availableStrategies);
            UpdateWithGameOptions(options);
        }

        public IEnumerable<KeyValuePair<string, bool>> StartingPlayerChoices
        {
            get; set;
        }
                
        public bool UserStartsNewGames
        {
            get { return _userStartsNewGames; }
            set
            {
                if (_userStartsNewGames != value)
                {
                    _userStartsNewGames = value;
                    Notify();
                }
            }
        }

        public ObservableCollection<string> AlgorithmChoices
        {
            get; set;
        }

        public string SelectedAlgorithm
        {
            get { return _selectedAlgorithm; }
            set
            {
                if (_selectedAlgorithm != value)
                {
                    _selectedAlgorithm = value;
                    Notify();
                }
            }
        }

        public IEnumerable<int> AlgorithmLevels
        {
            get; set;
        }

        public int SelectedLevel
        {
            get { return _selectedLevel; }
            set
            {
                if (_selectedLevel != value)
                {
                    _selectedLevel = value;
                    Notify();
                }
            }
        }

        public ICommand SaveOptionsCommand { get; }

        public ICommand UndoChangesCommand { get; }

        public IGameOptions ToGameOptions()
        {
            return new GameOptions()
            {
                UserPlaysAsBlack = UserStartsNewGames,
                StrategyName = SelectedAlgorithm,
                StrategyLevel = SelectedLevel
            };
        }

        private void UpdateWithGameOptions(IGameOptions options)
        {
            UserStartsNewGames = options.UserPlaysAsBlack;
            SelectedAlgorithm = options.StrategyName;
            SelectedLevel = options.StrategyLevel;
        }

        private void UpdateWithStrategies(IEnumerable<StrategyInfo> strategyInfos)
        {
            foreach(var strategy in strategyInfos)
            {
                if (!AlgorithmChoices.Contains(strategy.Name))
                {
                    AlgorithmChoices.Add(strategy.Name);
                }
            }
        }


        private void ExecuteUndoChanges(IDialogWindow window)
        {
            _dialogChoice = DialogChoice.Cancel;
            window.DialogResult = false;
        }

        private void ExecuteSaveOptions(IDialogWindow window)
        {
            _dialogChoice = DialogChoice.Ok;
            window.DialogResult = true;
        }

        public DialogChoice GetDialogChoice()
        {
            return _dialogChoice;
        }
    }
}
