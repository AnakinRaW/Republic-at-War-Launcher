using System;
using System.Diagnostics;

namespace RawLauncherWPF.Utilities
{
    public static class NotepadHelper
    {
        public static void ShowMessage(string message = null, string title = null)
        {
            var notepad = Process.Start(new ProcessStartInfo("notepad.exe"));
            if (notepad == null)
                return;
            notepad.WaitForInputIdle();

            if (!string.IsNullOrEmpty(title))
                NativeMethods.NativeMethods.SetWindowText(notepad.MainWindowHandle, title);

            if (string.IsNullOrEmpty(message))
                return;
            var child = NativeMethods.NativeMethods.FindWindowEx(notepad.MainWindowHandle, new IntPtr(0), "Edit", null);
            NativeMethods.NativeMethods.SendMessage(child, 0x000C, 0, message);
        }
    }
}
