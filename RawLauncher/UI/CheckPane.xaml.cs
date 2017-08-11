using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.UI
{
    /// <summary>
    /// Interaktionslogik für CheckPane.xaml
    /// </summary>
    public partial class CheckPane : ILauncherPane
    {
        public CheckPane(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            MainWindowViewModel = mainWindowViewModel;
            DataContext = new CheckViewModel(this);
            ViewModel = (LauncherPaneViewModel)DataContext;
            
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public LauncherPaneViewModel ViewModel { get; }
    }
}
