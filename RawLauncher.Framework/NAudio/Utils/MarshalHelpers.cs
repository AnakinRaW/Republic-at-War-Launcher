using System;
using System.Runtime.InteropServices;

namespace RawLauncher.Framework.NAudio.Utils
{
    public static class MarshalHelpers
    {       
        /// <summary>
        /// Pointer to Structure
        /// </summary>
        public static T PtrToStructure<T>(IntPtr pointer)
        {
            return (T)Marshal.PtrToStructure(pointer, typeof(T));
        }
    }
}
