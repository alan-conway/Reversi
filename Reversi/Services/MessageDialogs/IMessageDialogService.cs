using Reversi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Services.MessageDialogs
{
    public interface IMessageDialogService
    {
        DialogChoice ShowYesNoDialog(string title, string message);

        DialogChoice ShowOptionsDialog(IDialogViewModel dialogViewModel);
    }
}
