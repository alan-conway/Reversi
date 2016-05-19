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
        public DialogChoice ShowYesNoDialog(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo);
            return result == MessageBoxResult.Yes ? DialogChoice.Yes : DialogChoice.No;
        }
    }
}
