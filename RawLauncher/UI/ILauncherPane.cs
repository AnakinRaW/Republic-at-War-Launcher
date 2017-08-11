using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.UI
{
    public interface ILauncherPane
    {
        MainWindowViewModel MainWindowViewModel { get; }
        LauncherPaneViewModel ViewModel { get; }
    }
}