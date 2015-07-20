using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für RestorePane.xaml
    /// </summary>
    public partial class RestorePane : ILauncherPane
    {
        public RestorePane()
        {
            InitializeComponent();
            DataContext = new RestoreViewModel(this);
            ViewModel = (LauncherPaneViewModel) DataContext;
        }

        public LauncherPaneViewModel ViewModel { get; }
    }
}
