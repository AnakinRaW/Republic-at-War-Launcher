using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für LanguagePane.xaml
    /// </summary>
    public partial class LanguagePane : ILauncherPane
    {
        public LanguagePane()
        {
            InitializeComponent();
            DataContext = new LanguageViewModel(this);
            ViewModel = (LauncherPaneViewModel) DataContext;
        }

        public LauncherPaneViewModel ViewModel { get; }
    }
}
