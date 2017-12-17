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

        public void ActivateTab(int index)
        {
            switch (index)
            {
                case 0:
                    PlayTab.IsChecked = true;
                    break;
                case 1:
                    CheckTab.IsChecked = true;
                    break;
                case 2:
                    LangTab.IsChecked = true;
                    break;
                case 3:
                    RestoreTab.IsChecked = true;
                    break;
                case 4:
                    UpdateTab.IsChecked = true;
                    break;
            }
        }
    }
}
