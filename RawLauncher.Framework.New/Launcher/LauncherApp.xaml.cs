using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Caliburn.Micro;
using RawLauncher.Framework.Shell;

namespace RawLauncher.Framework.Launcher
{
    public partial class LauncherApp
    {
        private object ViewModel { get; }

        private static void LoadAssemblies()
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
            var mafExtractor = new ResourceExtractor.ResourceExtractor("Libraries");
            mafExtractor.ExtractFilesIfRequired(Directory.GetCurrentDirectory(),
                new[]
                {
                    "ModernApplicationFramework.dll"
                });
            LoadAssemblies();
        }
        
        public LauncherApp()
        {
            new Bootstrapper(false);
            // ViewModel = new LauncherViewModel();
        }
        
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var m = new WindowManager();
            m.ShowWindow(new MainWindowViewModel());
        }
    }
}
