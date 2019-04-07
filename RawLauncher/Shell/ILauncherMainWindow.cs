using System;
using System.Windows.Input;
using ModernApplicationFramework.Interfaces.ViewModels;
using RawLauncher.Framework.Versioning;

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

        ModVersion InstalledVersion { get; set; }

        ModVersion LatestVersion { get; set; }

        bool IsTestersBuild { get; set; }

        void ShowScreen(Type type);
    }
}