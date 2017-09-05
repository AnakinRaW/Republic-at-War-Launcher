using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Games
{
    public static class GameStartHelper
    {
        public static void StartGameProcess(Process process)
        {
            if (process == null)
                return;

            var fileName = process.StartInfo.FileName;
            var wd = process.StartInfo.WorkingDirectory;
            var a = process.StartInfo.Arguments;

            CreateShortcut(fileName, wd, a);

            var startingProcess = new Process
            {
                StartInfo = { FileName = Path.Combine(wd, "tmp.lnk") }
            };
            startingProcess.Start();

            Thread.Sleep(10000);

            File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "tmp.lnk"));
        }

        private static void CreateShortcut(string filePath, string path, string arguments)
        {
            var link = (NativeMethods.NativeMethods.IShellLink)new NativeMethods.NativeMethods.ShellLink();

            link.SetPath(filePath);
            link.SetWorkingDirectory(AppDomain.CurrentDomain.BaseDirectory);
            link.SetArguments(arguments);
            var exeFile = Path.Combine(Directory.GetCurrentDirectory(), "RawLauncher.Theme.dll");
            link.SetIconLocation(exeFile, 0);
            var file = (IPersistFile)link;
            file.Save(Path.Combine(path, "tmp.lnk"), false);
        }
    }
}
