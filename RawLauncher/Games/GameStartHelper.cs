using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace RawLauncher.Framework.Games
{
    public static class GameStartHelper
    {
        public static void StartGameProcess(Process process)
        {
            if (process == null)
                return;

            var fileName = process.StartInfo.FileName;
            var a = process.StartInfo.Arguments;

            var linkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "RaW_Modding_Team", "tmp.lnk");

            CreateShortcut(fileName, linkPath, a, process.StartInfo.WorkingDirectory);

            var startingProcess = new Process
            {
                StartInfo = { FileName = linkPath }
            };
            startingProcess.Start();
            Thread.Sleep(2000);
            //File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "tmp.lnk"));
        }

        private static void CreateShortcut(string filePath, string linkPath, string arguments, string wd)
        {
            var link = (NativeMethods.NativeMethods.IShellLink)new NativeMethods.NativeMethods.ShellLink();
            link.SetPath(filePath);
            link.SetWorkingDirectory(wd);
            link.SetArguments(arguments);
            var exeFile = Path.Combine(Directory.GetCurrentDirectory(), "RawLauncher.Theme.dll");
            link.SetIconLocation(exeFile, 0);
            var file = (IPersistFile)link;
            file.Save(linkPath, false);
        }
    }
}
