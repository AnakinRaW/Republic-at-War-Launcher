﻿using System.Data;
using System.Diagnostics;
using System.Windows;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using MainWindow = ModernApplicationFramework.Controls.MainWindow;

namespace RawLauncherWPF.ViewModels
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel
    {
        private readonly ILauncherPane _checkPane;
        private readonly ILauncherPane _languagePane;
        private readonly MainWindow _mainWindow;
        private readonly ILauncherPane _playPane;
        private readonly ILauncherPane _restorePane;
        private readonly ILauncherPane _updatePane;
        private ILauncherPane _activePane;

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
            _checkPane = new CheckPane(this);
            _languagePane = new LanguagePane(this);
            _restorePane = new RestorePane(this);
            _updatePane = new UpdatePane(this);

            PreSelectPane();
        }

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

        public LauncherViewModel LauncherViewModel { get; }

        private void PreSelectPane()
        {
            ActivePane = _playPane;
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

        public Command ShowPlayPaneCommand => new Command(ShowPlayPane);

        private void ShowPlayPane()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            ActivePane = _playPane;
        }

        public Command ShowCheckPaneCommand => new Command(ShowCheckPane);

        private void ShowCheckPane()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            ActivePane = _checkPane;
        }

        public Command ShowLanguagePaneCommand => new Command(ShowLanguage);

        private void ShowLanguage()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            ActivePane = _languagePane;
        }

        public Command ShowRestorePaneCommand => new Command(ShowRestorePane);

        private void ShowRestorePane()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            ActivePane = _restorePane;
        }

        public Command ShowUpdatePaneCommand => new Command(ShowUpdatePane);

        private void ShowUpdatePane()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            ActivePane = _updatePane;
        }

        protected override void Close()
        {
            _mainWindow.Close();
            base.Close();
        }

        #endregion
    }
}