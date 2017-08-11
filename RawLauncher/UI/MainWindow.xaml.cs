using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.UI
{
    public sealed partial class MainWindow
    {

        public LauncherViewModel LauncherViewModel { get; }

        public MainWindow(LauncherViewModel model)
        {
            LauncherViewModel = model;
            InitializeComponent();
            DataContext = new MainWindowViewModel(this, LauncherViewModel);
        }
        protected override bool ShouldAutoSize => false;
    }
}
