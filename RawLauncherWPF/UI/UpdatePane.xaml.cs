using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für UpdatePane.xaml
    /// </summary>
    public partial class UpdatePane : ILauncherPane
    {
        public UpdatePane(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            MainWindowViewModel = mainWindowViewModel;
            DataContext = new UpdateViewModel(this);
            ViewModel = (LauncherPaneViewModel)DataContext;
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public LauncherPaneViewModel ViewModel { get; }
    }
}
