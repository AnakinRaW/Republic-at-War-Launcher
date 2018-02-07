using System;
using System.Reflection;
using System.Windows;

namespace RawLauncher.Framework.Launcher
{
    public partial class LauncherApp
    {
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
            LoadAssemblies();
        }
        
        public LauncherApp()
        {
            new Bootstrapper(false);
        }
        
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            // StartUpLaunncher();  

            MessageBox.Show("Hello");
        }
    }
}
