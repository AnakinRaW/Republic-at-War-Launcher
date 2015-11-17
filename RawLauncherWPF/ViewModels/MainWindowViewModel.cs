using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.Themes.LauncherTheme;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.Utilities.MessageProvider;
using MainWindow = ModernApplicationFramework.Controls.MainWindow;

namespace RawLauncherWPF.ViewModels
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel
    {
        private readonly MainWindow _mainWindow;
        private readonly ILauncherPane _playPane;
        private ILauncherPane _activePane;
        private Version _installedVersion;
        private bool _isBlocked;
        private Version _latestVersion;
        private readonly int _startPaneIndex;

        public MainWindowViewModel(MainWindow mainWindow, LauncherViewModel model) : base(mainWindow)
        {
            if (model == null)
                throw new NoNullAllowedException(nameof(model));
            LauncherViewModel = model;
            _mainWindow = mainWindow;

            IsSimpleWindow = true;
            UseStatusBar = false;
            UseTitleBar = false;
            UseSimpleMovement = true;

            _playPane = new PlayPane(this);
            ILauncherPane checkPane = new CheckPane(this);
            ILauncherPane languagePane = new LanguagePane(this);
            ILauncherPane restorePane = new RestorePane(this);
            ILauncherPane updatePane = new UpdatePane(this);

            mainWindow.Loaded += MainWindow_Loaded;

            LauncherPanes = new List<ILauncherPane> {_playPane, checkPane, languagePane, restorePane, updatePane};

            _startPaneIndex = 0;
            if (LauncherViewModel.BaseGame == null || LauncherViewModel.Eaw == null)
            {
                Show(GetMessage("ErrorInitFailed"));
                IsBlocked = true;
            }
            if (LauncherViewModel.CurrentMod == null)
            {
                Show(GetMessage("ErrorInitFailedMod"));
                IsBlocked = true;
                updatePane.ViewModel.CanExecute = true;
                _startPaneIndex = 4;
            }
        }

        /// <summary>
        /// Contains the Launcher View Model
        /// </summary>
        public LauncherViewModel LauncherViewModel { get; }

        /// <summary>
        /// Contains the Active Pane
        /// </summary>
        public ILauncherPane ActivePane
        {
            get { return _activePane; }
            set
            {
                if (Equals(value, _activePane))
                    return;
                if (value == null)
                    return;
                 _playPane.ViewModel.IsActive = false;
                _activePane = value;
                _playPane.ViewModel.IsActive = true;
                OnPropertyChanged();
            }
        }

        public Version InstalledVersion
        {
            get { return _installedVersion; }
            set
            {
                _installedVersion = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Tells if there is a critical task running which shall prevent from performing other tasks
        /// </summary>
        public bool IsBlocked
        {
            get { return _isBlocked; }
            set
            {
                if (Equals(value, _isBlocked))
                    return;
                _isBlocked = value;
                LauncherPanes.ForEach(p => p.ViewModel.CanExecute = !_isBlocked);
                OnPropertyChanged();
            }
        }

        public Version LatestVersion
        {
            get { return _latestVersion; }
            set
            {
                _latestVersion = value;
                OnPropertyChanged();
            }
        }

        public List<ILauncherPane> LauncherPanes { get; }

        public void ShowPane(object index, bool hasAudio = false)
        {
            if (hasAudio)
                AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            ActivePane = LauncherPanes.ElementAt(Convert.ToInt32(index));
        }

        private void MainWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Theme = new LauncherTheme();
            ShowPane(_startPaneIndex);
            Configuration.Config.CurrentLanguage.Reload();
        }

        private void ShowPaneAudio(object index)
        {
            ShowPane(index, true);
        }

        #region Commands

        public Command OpenModdbCommand => new Command(OpenModdb);

        private void OpenModdb()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            Process.Start(Configuration.Config.ModdbPage);
        }

        public Command OpenEeawCommand => new Command(OpenEeaw);

        private void OpenEeaw()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            Process.Start(Configuration.Config.EeawForum);
        }

        public Command<object> ShowPaneAudioCommand => new Command<object>(ShowPaneAudio, CanShowPane);

        private bool CanShowPane(object arg)
        {
            return true;
        }

        public Command AboutCommand => new Command(ShowAboutWindow);

        private void ShowAboutWindow()
        {
            new AboutWindow().ShowDialog();
        }

        protected override void Close()
        {
            _mainWindow.Close();
            base.Close();
        }

        #endregion
    }
}