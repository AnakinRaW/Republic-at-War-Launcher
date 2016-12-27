using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RawLauncherWPF.Utilities
{
    public static class ThreadUtilities
    {
        public static  Task SleepThread(int ms)
        {
            return Task.Run(() => Thread.Sleep(ms));
        }

        public static void ThreadSaveShutdown()
        {
            ThreadStart ts = delegate
            {
                Application.Current.Dispatcher.BeginInvoke((Action)delegate {
                    Application.Current.Shutdown();
                });
            };
            var t = new Thread(ts);
            t.Start();
        }
    }
}
