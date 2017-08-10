using System;
using RawLauncher.Framework.NAudio.Utils;
using RawLauncher.Framework.NAudio.Wave.WaveFormats;
using RawLauncher.Framework.NAudio.Wave.WaveOutputs;

namespace RawLauncher.Framework.NAudio.Wave.SampleProviders
{
    public class SampleToWaveProvider16 : IWaveProvider
    {
        private readonly ISampleProvider _sourceProvider;
        private volatile float _volume;
        private float[] _sourceBuffer;

        /// <summary>
        /// Converts from an ISampleProvider (IEEE float) to a 16 bit PCM IWaveProvider.
        /// Number of channels and sample rate remain unchanged.
        /// </summary>
        /// <param name="sourceProvider">The input source provider</param>
        public SampleToWaveProvider16(ISampleProvider sourceProvider)
        {
            if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                throw new ArgumentException("Input source provider must be IEEE float", nameof(sourceProvider));
            if (sourceProvider.WaveFormat.BitsPerSample != 32)
                throw new ArgumentException("Input source provider must be 32 bit", nameof(sourceProvider));

            WaveFormat = new WaveFormat(sourceProvider.WaveFormat.SampleRate, 16, sourceProvider.WaveFormat.Channels);

            _sourceProvider = sourceProvider;
            _volume = 1.0f;
        }

        /// <inheritdoc />
        /// <summary>
        /// Reads bytes from this wave stream
        /// </summary>
        /// <param name="destBuffer">The destination buffer</param>
        /// <param name="offset">Offset into the destination buffer</param>
        /// <param name="numBytes">Number of bytes read</param>
        /// <returns>Number of bytes read.</returns>
        public int Read(byte[] destBuffer, int offset, int numBytes)
        {
            int samplesRequired = numBytes / 2;
            _sourceBuffer = BufferHelpers.Ensure(_sourceBuffer, samplesRequired);
            int sourceSamples = _sourceProvider.Read(_sourceBuffer, 0, samplesRequired);
            var destWaveBuffer = new WaveBuffer(destBuffer);

            int destOffset = offset / 2;
            for (int sample = 0; sample < sourceSamples; sample++)
            {
                // adjust volume
                float sample32 = _sourceBuffer[sample] * _volume;
                // clip
                if (sample32 > 1.0f)
                    sample32 = 1.0f;
                if (sample32 < -1.0f)
                    sample32 = -1.0f;
                destWaveBuffer.ShortBuffer[destOffset++] = (short)(sample32 * 32767);
            }

            return sourceSamples * 2;
        }
        public WaveFormat WaveFormat { get; }
    }
}
