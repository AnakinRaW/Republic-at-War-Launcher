using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using RawLauncher.Framework.NAudio.Utils;
using RawLauncher.Framework.NAudio.Wave.WaveFormats;
using RawLauncher.Framework.NAudio.Wave.WaveStreams;

namespace RawLauncher.Framework.NAudio.FileFormats.Wav
{
    internal class WaveFileChunkReader
    {
        private readonly bool _strictMode;
        private bool _isRf64;
        private readonly bool _storeAllChunks;
        private long _riffSize;

        public WaveFileChunkReader()
        {
            _storeAllChunks = true;
            _strictMode = false;
        }

        public void ReadWaveHeader(Stream stream)
        {
            DataChunkPosition = -1;
            WaveFormat = null;
            RiffChunks = new List<RiffChunk>();
            DataChunkLength = 0;

            var br = new BinaryReader(stream);
            ReadRiffHeader(br);
            _riffSize = br.ReadUInt32(); // read the file size (minus 8 bytes)

            if (br.ReadInt32() != ChunkIdentifier.ChunkIdentifierToInt32("WAVE"))
            {
                throw new FormatException("Not a WAVE file - no WAVE header");
            }

            if (_isRf64)
            {
                ReadDs64Chunk(br);
            }

            int dataChunkId = ChunkIdentifier.ChunkIdentifierToInt32("data");
            int formatChunkId = ChunkIdentifier.ChunkIdentifierToInt32("fmt ");
            
            // sometimes a file has more data than is specified after the RIFF header
            long stopPosition = Math.Min(_riffSize + 8, stream.Length);

            // this -8 is so we can be sure that there are at least 8 bytes for a chunk id and length
            while (stream.Position <= stopPosition - 8)
            {
                int chunkIdentifier = br.ReadInt32();
                var chunkLength = br.ReadUInt32();
                if (chunkIdentifier == dataChunkId)
                {
                    DataChunkPosition = stream.Position;
                    if (!_isRf64) // we already know the dataChunkLength if this is an RF64 file
                    {
                        DataChunkLength = chunkLength;
                    }
                    stream.Position += chunkLength;
                }
                else if (chunkIdentifier == formatChunkId)
                {
                    if (chunkLength > int.MaxValue)
                         throw new InvalidDataException($"Format chunk length must be between 0 and {int.MaxValue}.");
                    WaveFormat = WaveFormat.FromFormatChunk(br, (int)chunkLength);
                }
                else
                {
                    // check for invalid chunk length
                    if (chunkLength > stream.Length - stream.Position)
                    {
                        if (_strictMode)
                        {
                            Debug.Assert(false,
                                $"Invalid chunk length {chunkLength}, pos: {stream.Position}. length: {stream.Length}");
                        }
                        // an exception will be thrown further down if we haven't got a format and data chunk yet,
                        // otherwise we will tolerate this file despite it having corrupt data at the end
                        break;
                    }
                    if (_storeAllChunks)
                    {
                        if (chunkLength > int.MaxValue)
                            throw new InvalidDataException(
                                $"RiffChunk chunk length must be between 0 and {int.MaxValue}.");
                        RiffChunks.Add(GetRiffChunk(stream, chunkIdentifier, (int)chunkLength));
                    }
                    stream.Position += chunkLength;
                }
            }

            if (WaveFormat == null)
            {
                throw new FormatException("Invalid WAV file - No fmt chunk found");
            }
            if (DataChunkPosition == -1)
            {
                throw new FormatException("Invalid WAV file - No data chunk found");
            }
        }

        /// <summary>
        /// http://tech.ebu.ch/docs/tech/tech3306-2009.pdf
        /// </summary>
        private void ReadDs64Chunk(BinaryReader reader)
        {
            int ds64ChunkId = ChunkIdentifier.ChunkIdentifierToInt32("ds64");
            int chunkId = reader.ReadInt32();
            if (chunkId != ds64ChunkId)
                throw new FormatException("Invalid RF64 WAV file - No ds64 chunk found");
            int chunkSize = reader.ReadInt32();
            _riffSize = reader.ReadInt64();
            DataChunkLength = reader.ReadInt64();
            long sampleCount = reader.ReadInt64(); // replaces the value in the fact chunk
            reader.ReadBytes(chunkSize - 24); // get to the end of this chunk (should parse extra stuff later)
        }

        private static RiffChunk GetRiffChunk(Stream stream, int chunkIdentifier, int chunkLength)
        {
            return new RiffChunk(chunkLength, stream.Position);
        }

        private void ReadRiffHeader(BinaryReader br)
        {
            var header = br.ReadInt32();
            if (header == ChunkIdentifier.ChunkIdentifierToInt32("RF64"))
                _isRf64 = true;
            else if (header != ChunkIdentifier.ChunkIdentifierToInt32("RIFF"))
                throw new FormatException("Not a WAVE file - no RIFF header");
        }

        /// <summary>
        /// WaveFormat
        /// </summary>
        public WaveFormat WaveFormat { get; private set; }

        /// <summary>
        /// Data Chunk Position
        /// </summary>
        public long DataChunkPosition { get; private set; }

        /// <summary>
        /// Data Chunk Length
        /// </summary>
        public long DataChunkLength { get; private set; }

        /// <summary>
        /// Riff Chunks
        /// </summary>
        public List<RiffChunk> RiffChunks { get; private set; }
    }
}
