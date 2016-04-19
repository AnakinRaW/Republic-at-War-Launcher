using System.Windows.Interop;
using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow
    {

        public LauncherViewModel LauncherViewModel { get; }

        public MainWindow(LauncherViewModel model)
        {
            LauncherViewModel = model;
            InitializeComponent();
            DataContext = new MainWindowViewModel(this, LauncherViewModel);
            ChangeOwner(new WindowInteropHelper(this).Handle);
            ChangeOwnerForActivate(new WindowInteropHelper(this).Handle);        
        }

        protected override bool ShouldAutoSize => false;
    }
}
