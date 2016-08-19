using System;
using System.Diagnostics;
using System.Linq;

namespace RawLauncherWPF.Games
{
    public static class ProcessHelper
    {
        public static Process FindProcess(string name)
        {
            return Process.GetProcessesByName(name).FirstOrDefault();
        }

        public static bool IsProcessWithNameAlive(Process process)
        {
            try
            {
                return Process.GetProcesses().Any(x => x.ProcessName == process.ProcessName);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
