using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für RestorePane.xaml
    /// </summary>
    public partial class RestorePane : ILauncherPane
    {
        public RestorePane(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            MainWindowViewModel = mainWindowViewModel;
            DataContext = new RestoreViewModel(this);
            ViewModel = (LauncherPaneViewModel) DataContext;
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public LauncherPaneViewModel ViewModel { get; }
    }
}
