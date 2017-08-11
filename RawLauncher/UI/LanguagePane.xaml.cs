using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.UI
{
    public partial class LanguagePane : ILauncherPane
    {
        public LanguagePane(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            MainWindowViewModel = mainWindowViewModel;
            DataContext = new LanguageViewModel(this);
            ViewModel = (LauncherPaneViewModel)DataContext;
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public LauncherPaneViewModel ViewModel { get; }
    }
}
