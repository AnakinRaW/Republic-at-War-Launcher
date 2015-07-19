using System.Windows.Controls;
using System.Windows.Media;
using ModernApplicationFramework.Commands;
using RawLauncherWPF.UI;
using MainWindow = ModernApplicationFramework.Controls.MainWindow;

namespace RawLauncherWPF.ViewModels
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel
    {
        private readonly MainWindow _mainWindow;

        private ILauncherPane _activePane;

        public MainWindowViewModel(MainWindow mainWindow) : base(mainWindow)
        {
            _mainWindow = mainWindow;
            IsSimpleWindow = true;
            UseStatusBar = false;
            UseTitleBar = false;
            UseSimpleMovement = true;
        }


        public Border ActivePane => new Border() {Background = Brushes.Red};

        //public ILauncherPane ActivePane
        //{
        //    get { return _activePane; }
        //    set
        //    {
        //        if (Equals(value, _activePane))
        //            return;
        //        if (value == null)
        //            return;
        //        _activePane = value;
        //        OnPropertyChanged();
        //    }
        //}

        #region Commands

        public Command OpenModdbCommand => new Command(OpenModdb);

        private void OpenModdb()
        {
            throw new System.NotImplementedException();
        }

        public Command OpenEeawCommand => new Command(OpenEeaw);

        private void OpenEeaw()
        {
            throw new System.NotImplementedException();
        }

        public Command ShowPlayPaneCommand => new Command(ShowPlayPane);

        private void ShowPlayPane()
        {
            throw new System.NotImplementedException();
        }

        public Command ShowCheckPaneCommand => new Command(ShowCheckPane);

        private void ShowCheckPane()
        {
            throw new System.NotImplementedException();
        }

        public Command ShowLanguagePaneCommand => new Command(ShowLanguage);

        private void ShowLanguage()
        {
            throw new System.NotImplementedException();
        }

        public Command ShowRestorePaneCommand => new Command(ShowRestorePane);

        private void ShowRestorePane()
        {
            throw new System.NotImplementedException();
        }

        public Command ShowUpdatePaneCommand => new Command(ShowUpdatePane);

        private void ShowUpdatePane()
        {
            throw new System.NotImplementedException();
        }

        protected override void Close()
        {
            _mainWindow.Close();
            base.Close();
        }

        #endregion
    }
}
