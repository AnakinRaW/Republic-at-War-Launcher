using System.Runtime.InteropServices;

namespace RawLauncherWPF.NativeMethods
{
    class NativeMethods
    {
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool ComputerHasInternetConnection()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }
    }
}
