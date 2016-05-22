using Reversi.Services.MessageDialogs;

namespace Reversi.ViewModel
{
    public interface IDialogViewModel
    {
        DialogChoice GetDialogChoice();
    }
}