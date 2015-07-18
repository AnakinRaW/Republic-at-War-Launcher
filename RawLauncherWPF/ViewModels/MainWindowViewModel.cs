using System.Windows.Controls;
using System.Windows.Media;
using RawLauncherWPF.UI;
using MainWindow = ModernApplicationFramework.Controls.MainWindow;

namespace RawLauncherWPF.ViewModels
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel
    {
        private MainWindow _mainWindow;

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
    }
}
