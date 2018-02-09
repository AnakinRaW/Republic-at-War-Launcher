using System;
using System.Windows.Input;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace RawLauncher.Framework.Shell
{
    public interface ILauncherMainWindow : IMainWindowViewModel
    {
        ICommand OpenModdbCommand { get; }

        ICommand OpenEeawCommand { get; }

        ICommand AboutCommand { get; }

        ICommand DeleteRawCommand { get; }

        ICommand ShowPaneAudioCommand { get; }

        bool IsBlocked { get; set; }

        Version InstalledVersion { get; set; }

        Version LatestVersion { get; set; }

        void ShowScreen(Type type);
    }
}