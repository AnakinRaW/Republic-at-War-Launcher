using System.IO;
using System.Runtime.InteropServices;

namespace RawLauncher.Framework.NAudio.Wave.WaveFormats
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 2)]
    public class WaveFormatExtraData : WaveFormat
    {
        // try with 100 bytes for now, increase if necessary
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        private readonly byte[] extraData = new byte[100];

        internal WaveFormatExtraData()
        {
        }

        internal void ReadExtraData(BinaryReader reader)
        {
            if (extraSize > 0)
            {
                reader.Read(extraData, 0, extraSize);
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            if (extraSize > 0)
            {
                writer.Write(extraData, 0, extraSize);
            }
        }
    }
}
