using System.Data;
using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.UI
{
    /// <summary>
    /// Interaktionslogik für PlayPane.xaml
    /// </summary>
    public partial class PlayPane : ILauncherPane
    {
        public PlayPane(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            if (mainWindowViewModel == null)
                throw new NoNullAllowedException(nameof(mainWindowViewModel));
            MainWindowViewModel = mainWindowViewModel;
            DataContext = new PlayViewModel(this);
            ViewModel = (LauncherPaneViewModel)DataContext;
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public LauncherPaneViewModel ViewModel { get; }
    }
}
