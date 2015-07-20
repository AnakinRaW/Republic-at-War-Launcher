using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für UpdatePane.xaml
    /// </summary>
    public partial class UpdatePane : ILauncherPane
    {
        public UpdatePane()
        {
            InitializeComponent();
            DataContext = new UpdateViewModel(this);
            ViewModel = (LauncherPaneViewModel) DataContext;
        }

        public LauncherPaneViewModel ViewModel { get; }
    }
}
