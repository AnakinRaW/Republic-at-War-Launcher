using System;

namespace RawLauncher.Framework.AssemblyHelper
{
    public static class AssemblyLoader
    {
        internal static void LoadAssemblies()
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

        static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
