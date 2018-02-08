using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.WindowModels;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.ViewModels;
using RawLauncher.Framework.Controls;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.TestScreen;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Shell
{
    [Export(typeof(IMainWindowViewModel))]
    public class MainWindowViewModel : MainWindowViewModelConductorOneActive
    {
        private Version _installedVersion;
        private Version _latestVersion;

        public Command OpenModdbCommand => new Command(OpenModdb);

        public Command OpenEeawCommand => new Command(OpenEeaw);

        public Command AboutCommand => new Command(ShowAboutWindow);

        public Command DeleteRawCommand => new Command(DeleteRaw);

        // public Command<object> ShowPaneAudioCommand => new ObjectCommand(ShowPaneAudio, CanShowPane);

        public Version InstalledVersion
        {
            get => _installedVersion;
            set
            {
                _installedVersion = value;
                NotifyOfPropertyChange();
            }
        }

        public Version LatestVersion
        {
            get => _latestVersion;
            set
            {
                _latestVersion = value;
                NotifyOfPropertyChange();
            }
        }

        public override void OnImportsSatisfied()
        {
            base.OnImportsSatisfied();
            ActivateItem(IoC.Get<TestScreenViewModel>());
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            Application.Current.MainWindow = view as Window;
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            IsSimpleWindow = true;
            UseSimpleMovement = true;
            UseTitleBar = false;
            UseMenu = false;
            Configuration.Config.CurrentLanguage.Reload();

            var launcher = IoC.Get<LauncherModel>();
            InstalledVersion = launcher.CurrentMod == null ? new Version("1.0") : launcher.CurrentMod.Version;
            LatestVersion = VersionUtilities.GetLatestModVersion();
        }

        private static void OpenEeaw()
        {
            Process.Start(Configuration.Config.RaWHomepage);
        }

        private static void OpenModdb()
        {
            Process.Start(Configuration.Config.ModdbPage);
        }

        private static void ShowAboutWindow()
        {
            new AboutWindow().ShowDialog();
        }

        private static void DeleteRaw()
        {
            var result = MessageProvider.Show(MessageProvider.GetMessage("UninstallModWarning"), "Republic at War",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            if (result == MessageBoxResult.No)
                return;
            var launcher = IoC.Get<LauncherModel>();
            launcher.BaseGame.DeleteMod(launcher.CurrentMod);
            launcher.BaseGame.ClearDataFolder();
            launcher.BaseGame.Patch();
        }
    }
}
