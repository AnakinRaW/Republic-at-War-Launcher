using System.Diagnostics;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using MainWindow = ModernApplicationFramework.Controls.MainWindow;

namespace RawLauncherWPF.ViewModels
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel
    {
        private readonly MainWindow _mainWindow;

        private ILauncherPane _activePane;

        private readonly ILauncherPane _playPane;
        private readonly ILauncherPane _checkPane;
        private readonly ILauncherPane _languagePane;
        private readonly ILauncherPane _restorePane;
        private readonly ILauncherPane _updatePane;

        public MainWindowViewModel(MainWindow mainWindow) : base(mainWindow)
        {
            _mainWindow = mainWindow;
            IsSimpleWindow = true;
            UseStatusBar = false;
            UseTitleBar = false;
            UseSimpleMovement = true;

            _playPane = new PlayPane();
            _checkPane = new CheckPane();
            _languagePane = new LanguagePane();
            _restorePane = new RestorePane();
            _updatePane = new UpdatePane();
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
