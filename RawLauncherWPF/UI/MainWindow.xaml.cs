using System.Windows.Interop;
using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public LauncherViewModel LauncherViewModel { get; }

        public MainWindow(LauncherViewModel model)
        {
            LauncherViewModel = model;
            InitializeComponent();
            DataContext = new MainWindowViewModel(this, LauncherViewModel);
            this.ChangeOwner(new WindowInteropHelper(this).Handle);
            ChangeOwnerForActivate(new WindowInteropHelper(this).Handle);

        }
    }
}
