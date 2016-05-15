using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.UI.Tests.Extensions
{
    public static class NotifyPropertyChangedExtension
    {
        public static void OnNotify(this INotifyPropertyChanged viewModel, 
            string propertyName, Action action)
        {
            viewModel.PropertyChanged += (src, arg) =>
            {
                if (arg.PropertyName == propertyName)
                {
                    action();
                }
            };
        }
    }
}
