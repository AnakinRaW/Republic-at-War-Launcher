using System.Windows;
using System.Windows.Input;
using RawLauncherWPF.Server;
using static RawLauncherWPF.Models.LauncherModel;

namespace RawLauncherWPF.Launcher
{
    /// <summary>
    /// Interaktionslogik für "LauncherApp.xaml"
    /// </summary>
    public partial class LauncherApp
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow?.Show();         
        }
    }
}
