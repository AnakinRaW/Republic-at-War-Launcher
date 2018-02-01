using System.Data;
using System.IO;
using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.UI
{
    /// <inheritdoc cref="ILauncherPane" />
    public partial class PlayPane : ILauncherPane
    {
        public PlayPane(MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();
            MainWindowViewModel = mainWindowViewModel ?? throw new NoNullAllowedException(nameof(mainWindowViewModel));
            DataContext = new PlayViewModel(this);
            ViewModel = (LauncherPaneViewModel)DataContext;

            ToggleButton.IsChecked =
                File.Exists(Configuration.Config.RaWAppDataPath + Configuration.Config.FastLaunchFileName);
        }

        public MainWindowViewModel MainWindowViewModel { get; }
        public LauncherPaneViewModel ViewModel { get; }
    }
}
