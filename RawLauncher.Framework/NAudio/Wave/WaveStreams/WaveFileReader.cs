using System;
using System.Collections.Generic;
using System.IO;
using RawLauncher.Framework.NAudio.FileFormats.Wav;
using RawLauncher.Framework.NAudio.Wave.WaveFormats;

namespace RawLauncher.Framework.NAudio.Wave.WaveStreams 
{
    public class WaveFileReader : WaveStream
    {
        private readonly WaveFormat _waveFormat;
        private readonly bool _ownInput;
        private readonly long _dataPosition;
        private readonly long _dataChunkLength;
        private readonly object _lockObject = new object();
        private Stream _waveStream;

        /// <summary>Supports opening a WAV file</summary>
        /// <remarks>The WAV file format is a real mess, but we will only
        /// support the basic WAV file format which actually covers the vast
        /// majority of WAV files out there. For more WAV file format information
        /// visit www.wotsit.org. If you have a WAV file that can't be read by
        /// this class, email it to the NAudio project and we will probably
        /// fix this reader to support it
        /// </remarks>
        public WaveFileReader(String waveFile) :
            this(File.OpenRead(waveFile), true)
        {            
        }


        private WaveFileReader(Stream inputStream, bool ownInput)
        {
            _waveStream = inputStream;
            var chunkReader = new WaveFileChunkReader();
            try
            {
                chunkReader.ReadWaveHeader(inputStream);
                _waveFormat = chunkReader.WaveFormat;
                _dataPosition = chunkReader.DataChunkPosition;
                _dataChunkLength = chunkReader.DataChunkLength;
                ExtraChunks = chunkReader.RiffChunks;
            }
            catch
            {
                if (ownInput)
                {
                    inputStream.Dispose();
                }

                throw;
            }

            Position = 0;
            _ownInput = ownInput;
        }

        /// <summary>
        /// Gets a list of the additional chunks found in this file
        /// </summary>
        public List<RiffChunk> ExtraChunks { get; }

        /// <summary>
        /// Gets the data for the specified chunk
        /// </summary>
        public byte[] GetChunkData(RiffChunk chunk)
        {
            long oldPosition = _waveStream.Position;
            _waveStream.Position = chunk.StreamPosition;
            byte[] data = new byte[chunk.Length];
            _waveStream.Read(data, 0, data.Length);
            _waveStream.Position = oldPosition;
            return data;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Release managed resources.
                if (_waveStream != null)
                {
                    // only dispose our source if we created it
                    if (_ownInput)
                    {
                        _waveStream.Dispose();
                    }
                    _waveStream = null;
                }
            }
            else
            {
                System.Diagnostics.Debug.Assert(false, "WaveFileReader was not disposed");
            }
            // Release unmanaged resources.
            // Set large fields to null.
            // Call Dispose on your base class.
            base.Dispose(disposing);
        }

        public override WaveFormat WaveFormat => _waveFormat;

        public override long Length => _dataChunkLength;

        public sealed override long Position
        {
            get => _waveStream.Position - _dataPosition;
            set
            {
                lock (_lockObject)
                {
                    value = Math.Min(value, Length);
                    // make sure we don't get out of sync
                    value -= (value % _waveFormat.BlockAlign);
                    _waveStream.Position = value + _dataPosition;
                }
            }
        }

        public override int Read(byte[] array, int offset, int count)
        {
            if (count % _waveFormat.BlockAlign != 0)
            {
                throw new ArgumentException(
                    $"Must read complete blocks: requested {count}, block align is {WaveFormat.BlockAlign}");
            }
            lock (_lockObject)
            {
                // sometimes there is more junk at the end of the file past the data chunk
                if (Position + count > _dataChunkLength)
                {
                    count = (int) (_dataChunkLength - Position);
                }
                return _waveStream.Read(array, offset, count);
            }
        }
    }
}
