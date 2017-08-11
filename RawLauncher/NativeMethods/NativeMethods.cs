using System;
using System.Runtime.InteropServices;

namespace RawLauncher.Framework.NativeMethods
{
    public class NativeMethods
    {
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool ComputerHasInternetConnection()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }


        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        internal static extern int SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        internal static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
    }
}
