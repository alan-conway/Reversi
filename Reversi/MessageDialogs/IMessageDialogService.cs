using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.MessageDialogs
{
    public interface IMessageDialogService
    {
        DialogChoice ShowYesNoDialog(string title, string message);
    }
}
