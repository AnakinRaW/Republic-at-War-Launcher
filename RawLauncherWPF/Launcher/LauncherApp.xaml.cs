using System.Windows;
using System.Windows.Input;
using RawLauncherWPF.Server;

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
        async private void StartUpLaunncher()
        {
            // If "RaW.txt" does exists AND Shift is NOT pressed -> Show UpdateScreen and Run Mod afterwards
            // Else Run MainWindow (which inits the the Update View which checks for update on creation)
            if (_launcherViewModel.FastLaunchFileExists && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                await _launcherViewModel.FastLaunchUpdateSearchCommand.Execute();
                await _launcherViewModel.StartModCommand.Execute();
                Shutdown(0);
                return;
            }
            if (_launcherViewModel.FastLaunchFileExists)
                await _launcherViewModel.DeleteFastLaunchFileCommand.Execute();
            await _launcherViewModel.ShowMainWindowCommand.Execute();
        }
    }
}
