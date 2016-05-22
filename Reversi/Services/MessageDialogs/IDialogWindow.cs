using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Services.MessageDialogs
{
    public interface IDialogWindow
    {
        bool? ShowDialog();
        bool? DialogResult { set; }
        object DataContext { set; }
        
    }
}
