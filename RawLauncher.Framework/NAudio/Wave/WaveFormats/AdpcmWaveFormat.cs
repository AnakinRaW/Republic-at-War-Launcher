using System.Runtime.InteropServices;

namespace RawLauncher.Framework.NAudio.Wave.WaveFormats
{
    [StructLayout(LayoutKind.Sequential, Pack=2)]
    public abstract class AdpcmWaveFormat : WaveFormat
    {
        readonly short samplesPerBlock;

        readonly short numCoeff;
        // 7 pairs of coefficients
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)] readonly short[] coefficients;

        protected AdpcmWaveFormat(int sampleRate, int channels) :
            base(sampleRate,0,channels)
        {
            waveFormatTag = WaveFormatEncoding.Adpcm;
            
            // TODO: validate sampleRate, bitsPerSample
            extraSize = 32;
            switch(this.sampleRate)
            {
                case 8000: 
                case 11025:
                    blockAlign = 256; 
                    break;
                case 22050:
                    blockAlign = 512;
                    break;
                default:
                    blockAlign = 1024;
                    break;
            }

            bitsPerSample = 4;
            samplesPerBlock = (short) ((blockAlign - (7 * channels)) * 8 / (bitsPerSample * channels) + 2);
            averageBytesPerSecond =
                (SampleRate * blockAlign) / samplesPerBlock;

            // samplesPerBlock = blockAlign - (7 * channels)) * (2 / channels) + 2;


            numCoeff = 7;
            coefficients = new short[] {
                256,0,512,-256,0,0,192,64,240,0,460,-208,392,-232
            };
        }

        public override void Serialize(System.IO.BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(samplesPerBlock);
            writer.Write(numCoeff);
            foreach (short coefficient in coefficients)
            {
                writer.Write(coefficient);
            }
        }

        public override string ToString()
        {
            return
                $"Microsoft ADPCM {SampleRate} Hz {channels} channels {bitsPerSample} bits per sample {samplesPerBlock} samples per block";
        }
    }
}
