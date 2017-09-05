using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using ModernApplicationFramework.Input.Base;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Services;
using RawLauncher.Framework.UI;
using RawLauncher.Framework.Utilities;
using RawLauncher.Theme;
using AboutWindow = RawLauncher.Framework.UI.AboutWindow;
using Action = System.Action;
using CheckPane = RawLauncher.Framework.UI.CheckPane;
using LanguagePane = RawLauncher.Framework.UI.LanguagePane;
using PlayPane = RawLauncher.Framework.UI.PlayPane;
using RestorePane = RawLauncher.Framework.UI.RestorePane;
using UpdatePane = RawLauncher.Framework.UI.UpdatePane;

namespace RawLauncher.Framework.ViewModels
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.Controls.Windows.MainWindowViewModel
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
            LauncherViewModel = model;

            if (model == null)
                throw new NoNullAllowedException(nameof(model));

            _mainWindow = mainWindow;

            IsSimpleWindow = true;
            UseTitleBar = false;
            UseSimpleMovement = true;
            UseMenu = false;

            _playPane = new PlayPane(this);
            ILauncherPane checkPane = new CheckPane(this);
            ILauncherPane languagePane = new LanguagePane(this);
            ILauncherPane restorePane = new RestorePane(this);
            ILauncherPane updatePane = new UpdatePane(this);

            mainWindow.Loaded += MainWindow_Loaded;
            LauncherPanes = new List<ILauncherPane> {_playPane, checkPane, languagePane, restorePane, updatePane};
            _startPaneIndex = 0;
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
            get => _activePane;
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
            get => _installedVersion;
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
            get => _isBlocked;
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
            get => _latestVersion;
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IoC.Get<IThemeManager>().Theme = new LauncherTheme();
            IoC.Get<IStatusBarDataModelService>().SetVisibility(0);
            ShowPane(_startPaneIndex);
            Configuration.Config.CurrentLanguage.Reload();

            if (LauncherViewModel.BaseGame == null || LauncherViewModel.Eaw == null)
            {    
                IsBlocked = true;
                ShowPane(0);
                _mainWindow.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(() =>
                {
                    MessageProvider.ShowError(MessageProvider.GetMessage("ErrorInitFailed"));
                }));
            }
            if (LauncherViewModel.CurrentMod == null && LauncherViewModel.BaseGame != null && LauncherViewModel.Eaw != null)
            {
                IsBlocked = true;
                LauncherPanes.First(x => x.GetType() == typeof(UpdatePane)).ViewModel.CanExecute = true;
                ShowPane(4);
                _mainWindow.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, (Action)(() =>
                {
                    MessageProvider.ShowInformation(MessageProvider.GetMessage("ErrorInitFailedMod"));
                }));        
            }
        }

        private void ShowPaneAudio(object index)
        {
            ShowPane(index, true);
        }

        #region Commands

        public Command OpenModdbCommand => new Command(OpenModdb);

        private void OpenModdb()
        {
            Process.Start(Configuration.Config.ModdbPage);
        }

        public Command OpenEeawCommand => new Command(OpenEeaw);

        private void OpenEeaw()
        {
            Process.Start(Configuration.Config.EeawForum);
        }

        public Command<object> ShowPaneAudioCommand => new ObjectCommand(ShowPaneAudio, CanShowPane);

        private bool CanShowPane(object arg)
        {
            return true;
        }

        public Command AboutCommand => new Command(ShowAboutWindow);

        private void ShowAboutWindow()
        {
            new AboutWindow().ShowDialog();
        }

        public Command DeleteRawCommand => new Command(DeleteRaw);

        private void DeleteRaw()
        {
            var result = MessageProvider.Show(MessageProvider.GetMessage("UninstallModWarning"), "Republic at War",
                  MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            if (result == MessageBoxResult.No)
                return;
            LauncherViewModel.BaseGame.DeleteMod(LauncherViewModel.CurrentMod);
            LauncherViewModel.BaseGame.ClearDataFolder();
            LauncherViewModel.BaseGame.Patch();
        }

        protected override void Close()
        {
            _mainWindow.Close();
            base.Close();
        }

        #endregion
    }
}