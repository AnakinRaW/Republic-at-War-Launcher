using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RawLauncher.Framework.Utilities
{
    public static class ThreadUtilities
    {
        public static  Task SleepThread(int ms)
        {
            return Task.Run(() => Thread.Sleep(ms));
        }

        public static void ThreadSaveShutdown()
        {
            var t = new Thread(Ts);
            t.Start();

            void Ts()
            {
                Application.Current.Dispatcher.BeginInvoke((Action)delegate { Application.Current.Shutdown(); });
            }
        }
    }
}
