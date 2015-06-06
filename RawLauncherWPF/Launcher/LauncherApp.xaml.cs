using System.Windows;
using RawLauncherWPF.Server;
using static RawLauncherWPF.Launcher.LauncherDataMiner;

namespace RawLauncherWPF.Launcher
{
    /// <summary>
    /// Interaktionslogik für "LauncherApp.xaml"
    /// </summary>
    public partial class LauncherApp
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            CheckUpdateOnStartup();
            MainWindow?.Show();
        }

        private void CheckUpdateOnStartup()
        {
            try
            {
                DataMiner.HostServer.CheckForUpdate(DataMiner.CurrentMod.Version);
            }
            catch (ServerException)
            {
            } 
        }
    }
}
