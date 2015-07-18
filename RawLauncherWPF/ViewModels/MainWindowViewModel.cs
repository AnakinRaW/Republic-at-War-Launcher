using ModernApplicationFramework.Controls;

namespace RawLauncherWPF.ViewModels
{
    public sealed class MainWindowViewModel : ModernApplicationFramework.ViewModels.MainWindowViewModel
    {
        private MainWindow _mainWindow;

        public MainWindowViewModel(MainWindow mainWindow) : base(mainWindow)
        {
            _mainWindow = mainWindow;
            IsSimpleWindow = true;
            UseStatusBar = false;
            UseTitleBar = false;
            UseSimpleMovement = true;
        }
    }
}
