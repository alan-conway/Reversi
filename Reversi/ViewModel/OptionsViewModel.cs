using Prism.Commands;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reversi.ViewModel
{
    public class OptionsViewModel : ViewModelBase
    {
        private IGameEngine _engine;
        private bool _userPlaysAsBlack;

        public OptionsViewModel(IGameEngine engine)
        {
            _engine = engine;
            SaveOptionsCommand = new DelegateCommand(ExecuteSaveOptions, DifferencesFound);
            UndoChangesCommand = new DelegateCommand(ExecuteUndoChanges, DifferencesFound);

            UserColorChoices = new[] {
                new KeyValuePair<string, bool>("Black", true),
                new KeyValuePair<string, bool>("White", false) };

            InitialiseOptionsFromEngine();
        }

        public IEnumerable<KeyValuePair<string, bool>> UserColorChoices
        {
            get; set;
        }

        public bool UserPlaysAsBlack
        {
            get { return _userPlaysAsBlack; }
            set
            {
                if (_userPlaysAsBlack != value)
                {
                    _userPlaysAsBlack = value;
                    Notify();
                    NotifyButtons();
                }
            }
        }
        
        public ICommand SaveOptionsCommand { get; }

        public ICommand UndoChangesCommand { get; }

        private void ExecuteSaveOptions()
        {
            _engine.GameOptions = new GameOptions()
            {
                UserPlaysAsBlack = UserPlaysAsBlack
            };
            InitialiseOptionsFromEngine();
        }

        private void ExecuteUndoChanges()
        {
            InitialiseOptionsFromEngine();
        }
        
        private bool DifferencesFound()
        {
            return _engine.GameOptions.UserPlaysAsBlack != UserPlaysAsBlack;
        }     

        private void InitialiseOptionsFromEngine()
        {
            UserPlaysAsBlack = _engine.GameOptions.UserPlaysAsBlack;
            NotifyButtons();
        }

        private void NotifyButtons()
        {
            (SaveOptionsCommand as DelegateCommand).RaiseCanExecuteChanged();
            (UndoChangesCommand as DelegateCommand).RaiseCanExecuteChanged();
        }


    }
}
