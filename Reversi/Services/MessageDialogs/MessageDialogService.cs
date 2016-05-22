using Reversi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Reversi.Services.MessageDialogs
{
    public class MessageDialogService : IMessageDialogService
    {
        private Func<IDialogWindow> _optionsWindowFactory;

        public MessageDialogService(Func<IDialogWindow> optionsWindowFactory)
        {
            _optionsWindowFactory = optionsWindowFactory;
        }

        public DialogChoice ShowYesNoDialog(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes ? DialogChoice.Yes : DialogChoice.No;
        }

        public DialogChoice ShowOptionsDialog(IDialogViewModel viewModel)
        {
            var window = _optionsWindowFactory();
            window.DataContext = viewModel;
            window.ShowDialog();
            return viewModel.GetDialogChoice();
        }




    }
}
