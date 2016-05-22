using Prism.Commands;
using Reversi.Engine.Core;
using Reversi.Engine.Interfaces;
using Reversi.Services.MessageDialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reversi.ViewModel
{
    public class OptionsViewModel : ViewModelBase, IDialogViewModel
    {
        private bool _userPlaysAsBlack;
        private DialogChoice _dialogChoice;

        public OptionsViewModel(IGameOptions options)
        {
            _dialogChoice = DialogChoice.Cancel;

            SaveOptionsCommand = new DelegateCommand<IDialogWindow>(ExecuteSaveOptions);
            UndoChangesCommand = new DelegateCommand<IDialogWindow>(ExecuteUndoChanges);

            UserColorChoices = new[] {
                new KeyValuePair<string, bool>("Black", true),
                new KeyValuePair<string, bool>("White", false) };

            FromGameOptions(options);
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
                }
            }
        }
        
        public ICommand SaveOptionsCommand { get; }

        public ICommand UndoChangesCommand { get; }

        public IGameOptions ToGameOptions()
        {
            return new GameOptions()
            {
                UserPlaysAsBlack = UserPlaysAsBlack
            };
        }

        private void FromGameOptions(IGameOptions options)
        {
            UserPlaysAsBlack = options.UserPlaysAsBlack;
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
