using System.Windows;
using System.Windows.Input;
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
            // If "RaW.txt" does exists AND Shift is NOT pressed -> Show UpdateScreen and Run afterwards
            // Else Run MainWindow (which inits the the Update View which checks for update on creation)
            if (DataMiner.QuietLaunchFileExists && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                
            }
            MainWindow?.Show();         
        }

        public void TestAsync()
        {
        }

        private void CheckUpdateAsnyc()
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
