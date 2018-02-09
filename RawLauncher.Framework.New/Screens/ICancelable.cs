using System.Windows.Input;

namespace RawLauncher.Framework.Screens
{
    public interface ICancelable
    {
        ICommand CancelCommand { get; }

        void Cancel();
    }
}