using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.Launcher
{
    /// <summary>
    /// Interaktionslogik für "LauncherApp.xaml"
    /// </summary>
    public partial class LauncherApp
    {
        public object DataContext { get; }

        private readonly LauncherViewModel _launcherViewModel;


        public LauncherApp()
        {
            DataContext = new LauncherViewModel(this);
            _launcherViewModel = (LauncherViewModel) DataContext;
        }

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            StartUpLaunncher();  
        }


        /// <summary>
        /// This Method contains some actions that shall be performed after the launcher is ready to launch but before showing up
        /// Can Close Application after this compleded tasks.
        /// Runs CleanUp on Exit
        /// </summary>
        private async void StartUpLaunncher()
        {
            // If "RaW.txt" does exists AND Shift is NOT pressed -> Show UpdateScreen and Run Mod afterwards
            // Else Run MainWindow (which inits the the Update View which checks for update on creation)
            if (_launcherViewModel.FastLaunchFileExists && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                await _launcherViewModel.FastLaunchCommand.Execute();
                return;
            }
            if (_launcherViewModel.FastLaunchFileExists)
                await _launcherViewModel.DeleteFastLaunchFileCommand.Execute();
            await _launcherViewModel.NormalLaunchCommand.Execute();
        }

        public static void Play()
        {
            var startInfo = new ProcessStartInfo();
            //startInfo.FileName = Directory.GetParent(Directory.GetCurrentDirectory()) + "\\GameData\\sweaw.exe";
            startInfo.FileName = Registry.CurrentUser.CreateSubKey("Software\\Valve\\Steam", RegistryKeyPermissionCheck.ReadSubTree).GetValue("SteamExe", null).ToString();
            startInfo.Arguments = "-applaunch 32470 swfoc MODPATH=Mods\\Republic_at_War";
            string str = Directory.GetParent(new DirectoryInfo(Directory.GetCurrentDirectory()).FullName).FullName;
            File.Move(str + "\\runme.dat", str + "\\tmp.runme.dat.tmp");
            File.Copy(str + "\\runm2.dat", str + "\\runme.dat");
            Process.Start(startInfo);
            Thread.Sleep(20000);
            File.Delete(str + "\\runme.dat");
            File.Move(str + "\\tmp.runme.dat.tmp", str + "\\runme.dat");
        }
    }
}
