using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Services;
using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.Launcher
{
    public partial class LauncherApp
    {
        public object DataContext { get; }

        private readonly LauncherViewModel _launcherViewModel;
        
        public static void LoadAssemblies()
        {
            EmbeddedAssembly.Load("RawLauncher.Framework.Libraries.System.Windows.Interactivity.dll", "System.Windows.Interactivity.dll");
            EmbeddedAssembly.Load("RawLauncher.Framework.Libraries.NAudio.dll", "NAudio.dll");
            EmbeddedAssembly.Load("RawLauncher.Framework.Libraries.Caliburn.Micro.dll", "Caliburn.Micro.dll");
            EmbeddedAssembly.Load("RawLauncher.Framework.Libraries.Caliburn.Micro.Platform.Core.dll", "Caliburn.Micro.Platform.Core.dll");
            EmbeddedAssembly.Load("RawLauncher.Framework.Libraries.Caliburn.Micro.Platform.dll", "Caliburn.Micro.Platform.dll");
            EmbeddedAssembly.Load("RawLauncher.Framework.Libraries.ModernApplicationFramework.Utilities.dll", "ModernApplicationFramework.Utilities.dll");
            EmbeddedAssembly.Load("RawLauncher.Framework.Libraries.ModernApplicationFramework.dll", "ModernApplicationFramework.dll");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }

        static LauncherApp()
        {
            LoadAssemblies();
        }
        
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
