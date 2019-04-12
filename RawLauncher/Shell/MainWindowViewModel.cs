using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.WindowModels;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.Controls;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Screens;
using RawLauncher.Framework.Screens.PlayScreen;
using RawLauncher.Framework.Screens.UpdateScreen;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Shell
{
    [Export(typeof(ILauncherMainWindow))]
    public class MainWindowViewModel : MainWindowViewModelConductorOneActive, ILauncherMainWindow
    {
        private readonly ILauncherScreen[] _screens;
        private ModVersion _installedVersion;
        private ModVersion _latestVersion;
        private bool _isBlocked;
        private MainWindowView _window;
        private bool _isTestersBuild;

        public ICommand OpenModdbCommand => new Command(OpenModdb);

        public ICommand OpenEeawCommand => new Command(OpenEeaw);

        public ICommand AboutCommand => new Command(ShowAboutWindow);

        public ICommand DeleteRawCommand => new Command(DeleteRaw);

        public ICommand ShowPaneAudioCommand => new DelegateCommand(ShowPaneAudio);

        private void ShowPaneAudio(object obj)
        {
            if (!(obj is Type type))
                return;
            AudioPlayer.PlayAudio(AudioPlayer.Audio.ButtonPress);
            ShowScreen(type);
        }

        public ModVersion InstalledVersion
        {
            get => _installedVersion;
            set
            {
                _installedVersion = value;
                NotifyOfPropertyChange();
            }
        }

        public ModVersion LatestVersion
        {
            get => _latestVersion;
            set
            {
                _latestVersion = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsTestersBuild
        {
            get => _isTestersBuild;
            set
            {
                if (value == _isTestersBuild)
                    return;
                _isTestersBuild = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Tells if there is a critical task running which shall prevent from performing other tasks
        /// </summary>
        public bool IsBlocked
        {
            get => _isBlocked;
            set
            {
                if (Equals(value, _isBlocked))
                    return;
                _isBlocked = value;
                _screens.ForEach(p => p.CanExecute = !_isBlocked);
                NotifyOfPropertyChange();
            }
        }

        [ImportingConstructor]
        public MainWindowViewModel([ImportMany] ILauncherScreen[] screens, LauncherModel launcher, IModVersionWatcher versionWatcher)
        {
            versionWatcher.ModVersionChanged += VersionWatcher_ModVersionChanged;
            _screens = screens;
            if (launcher.CurrentMod == null)
                InstalledVersion = ModVersion.Parse(DummyMod.VersionName);
            else
                InstalledVersion = launcher.CurrentMod?.Version;
        }

        private void VersionWatcher_ModVersionChanged(object sender, ModVersion e)
        {
            InstalledVersion = e;
        }

        public void ShowScreen(Type type)
        {
            var model = IoC.GetInstance(type, null);
            ActivateItem(model);
            _window.ActivateTab(type);
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
            //InstalledVersion = launcher.CurrentMod == null ? ModVersion.Parse("1.0") : launcher.CurrentMod.Version;
            LatestVersion = VersionUtilities.GetLatestModVersion();
        }

        protected override void OnViewLoaded(object view)
        {
            _window = view as MainWindowView;
            var launcher = IoC.Get<LauncherModel>();
            if (launcher.BaseGame == null || launcher.Eaw == null)
            {
                IsBlocked = true;
                ShowScreen(typeof(IPlayScreen));
            }
            if (launcher.CurrentMod == null && launcher.BaseGame != null && launcher.Eaw != null)
            {
                IsBlocked = true;
                _screens.First(x => x is IUpdateScreen).CanExecute = true;
                ShowScreen(typeof(IUpdateScreen));
                OnUIThread(() =>
                {
                    MessageProvider.ShowInformation(MessageProvider.GetMessage("ErrorInitFailedMod"));
                });
            }
            else
                ShowScreen(typeof(IPlayScreen));
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
