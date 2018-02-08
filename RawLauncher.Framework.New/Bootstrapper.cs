using System;
using System.IO;
using System.Reflection;
using ModernApplicationFramework.Native.TrinetCoreNtfs;

namespace RawLauncher.Framework
{
    internal class Bootstrapper : ModernApplicationFramework.Basics.Services.Bootstrapper
    {
        public Bootstrapper(bool useApplication) : base(useApplication)
        {
            Initialize();
        }

        //TODO: Make this work again
        protected override void PreInitialize()
        {
            var exassemlby = Assembly.GetExecutingAssembly().Location;
            var directory = Path.GetDirectoryName(exassemlby);
            foreach (var filePath in Directory.EnumerateFiles(directory ?? 
                throw new InvalidOperationException(), "*.dll", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(filePath);
                fileInfo.DeleteAlternateDataStream("Zone.Identifier");
            }
        }
    }
}
