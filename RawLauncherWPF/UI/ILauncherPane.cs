using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    public interface ILauncherPane
    {
         LauncherPaneViewModel ViewModel { get; }
    }
}