using System.Threading;
using System.Threading.Tasks;

namespace RawLauncherWPF.Utilities
{
    public static class ThreadUtilities
    {
        public static  Task SleepThread(int ms)
        {
            return Task.Run(() => Thread.Sleep(ms));
        }
    }
}
