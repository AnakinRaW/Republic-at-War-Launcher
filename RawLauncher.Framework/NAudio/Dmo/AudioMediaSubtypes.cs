using System;

namespace RawLauncher.Framework.NAudio.Dmo
{
    internal static class AudioMediaSubtypes
    {
        public static readonly Guid MediasubtypePcm = new Guid("00000001-0000-0010-8000-00AA00389B71"); // PCM audio. 
        public static readonly Guid MediasubtypeIeeeFloat = new Guid("00000003-0000-0010-8000-00aa00389b71"); // Corresponds to WAVE_FORMAT_IEEE_FLOAT 
    }
}
