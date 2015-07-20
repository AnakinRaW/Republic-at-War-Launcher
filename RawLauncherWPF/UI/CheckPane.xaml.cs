using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für CheckPane.xaml
    /// </summary>
    public partial class CheckPane : ILauncherPane
    {
        public CheckPane()
        {
            InitializeComponent();
            DataContext = new CheckViewModel(this);
            ViewModel = (LauncherPaneViewModel) DataContext;
        }

        public LauncherPaneViewModel ViewModel { get; }
    }
}
