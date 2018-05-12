using System.Windows.Input;

namespace RawLauncher.Framework.Screens
{
    public interface IHasSelection
    {
        ICommand ChangeSelectionCommand { get; }
    }
}