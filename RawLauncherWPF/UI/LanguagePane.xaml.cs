using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für LanguagePane.xaml
    /// </summary>
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
