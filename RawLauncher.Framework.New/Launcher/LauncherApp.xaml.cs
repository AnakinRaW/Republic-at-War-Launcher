using System.IO;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.ViewModels;
using RawLauncher.Framework.AssemblyHelper;
using RawLauncher.Theme;

namespace RawLauncher.Framework.Launcher
{
    public partial class LauncherApp
    {
        static LauncherApp()
        {
            var mafExtractor = new AssemblyHelper.ResourceExtractor.ResourceExtractor("Libraries");
            mafExtractor.ExtractFilesIfRequired(Directory.GetCurrentDirectory(), "ModernApplicationFramework.dll");
            AssemblyLoader.LoadAssemblies();
        }
        
        public LauncherApp()
        {
            new Bootstrapper(false);
        }

        /// <summary>
        /// This Method contains some actions that shall be performed after the launcher is ready to launch but before showing up
        /// Can Close Application after this compleded tasks.
        /// Runs CleanUp on Exit
        /// </summary>
        private async void App_OnStartup(object sender, StartupEventArgs e)
        {
            var launcher = IoC.Get<LauncherModel>();

            // If "RaW.txt" does exists AND Shift is NOT pressed -> Show UpdateScreen and Run Mod afterwards
            // Else Run MainWindow (which inits the the Update View which checks for update on creation)
            if (launcher.FastLaunchFileExists && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                await launcher.FastLaunchCommand.Execute();
                return;
            }

            if (launcher.FastLaunchFileExists)
                await launcher.DeleteFastLaunchFileCommand.Execute();
            IoC.Get<IThemeManager>().Theme = new LauncherTheme();
            IoC.Get<IStatusBarDataModelService>().SetVisibility(0);
            ShowMainWindow();
            await launcher.NormalLaunchCommand.Execute();
        }

        private void ShowMainWindow()
        {
            var wm = IoC.Get<IWindowManager>();
            wm.ShowWindow(IoC.Get<IMainWindowViewModel>());
        }
    }
}
