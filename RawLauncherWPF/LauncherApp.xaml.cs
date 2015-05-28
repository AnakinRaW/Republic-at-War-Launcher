using System.Windows;

namespace RawLauncherWPF
{
    /// <summary>
    /// Interaktionslogik für "LauncherApp.xaml"
    /// </summary>
    public partial class LauncherApp
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var wnd = new MainWindow();
            wnd.Show();
        }
    }
}
