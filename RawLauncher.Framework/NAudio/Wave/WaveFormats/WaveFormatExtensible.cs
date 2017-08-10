using System;
using System.Runtime.InteropServices;

namespace RawLauncher.Framework.NAudio.Wave.WaveFormats
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]	
    public abstract class WaveFormatExtensible : WaveFormat
    {
        readonly short wValidBitsPerSample; // bits of precision, or is wSamplesPerBlock if wBitsPerSample==0
        readonly int dwChannelMask; // which channels are present in stream
        private Guid subFormat;

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(wValidBitsPerSample);
            writer.Write(dwChannelMask);
            byte[] guid = subFormat.ToByteArray();
            writer.Write(guid, 0, guid.Length);
        }

        public override string ToString()
        {
            return
                $"{base.ToString()} wBitsPerSample:{wValidBitsPerSample} dwChannelMask:{dwChannelMask} subFormat:{subFormat} extraSize:{extraSize}";
        }
    }
}
