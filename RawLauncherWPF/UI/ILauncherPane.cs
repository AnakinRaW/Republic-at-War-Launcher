using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    public interface ILauncherPane
    {
        MainWindowViewModel MainWindowViewModel { get; }
        LauncherPaneViewModel ViewModel { get; }
    }
}