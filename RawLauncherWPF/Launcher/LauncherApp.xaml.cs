using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Services;
using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.Launcher
{
    public partial class LauncherApp
    {
        public object DataContext { get; }

        private readonly LauncherViewModel _launcherViewModel;


        public LauncherApp()
        {
            new Bootstrapper(false);
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
    }
}
