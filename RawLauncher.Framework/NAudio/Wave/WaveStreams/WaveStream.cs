using System;
using System.IO;
using RawLauncher.Framework.NAudio.Wave.WaveFormats;
using RawLauncher.Framework.NAudio.Wave.WaveOutputs;

namespace RawLauncher.Framework.NAudio.Wave.WaveStreams 
{
    public abstract class WaveStream : Stream, IWaveProvider
    {
        public abstract WaveFormat WaveFormat { get; }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override void Flush() { }
        
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Begin)
                Position = offset;
            else if (origin == SeekOrigin.Current)
                Position += offset;
            else
                Position = Length + offset;
            return Position;
        }

        public override void SetLength(long length)
        {
            throw new NotSupportedException("Can't set length of a WaveFormatString");
        }
        
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Can't write to a WaveFormatString");
        }
    }
}
