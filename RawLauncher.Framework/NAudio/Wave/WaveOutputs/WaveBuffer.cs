using System.Runtime.InteropServices;

namespace RawLauncher.Framework.NAudio.Wave.WaveOutputs
{
    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    public class WaveBuffer : IWaveBuffer
    {
        /// <summary>
        /// Number of Bytes
        /// </summary>
        [FieldOffset(0)] private int numberOfBytes;
        [FieldOffset(8)]
        private byte[] byteBuffer;
        [FieldOffset(8)]
        private float[] floatBuffer;
        [FieldOffset(8)]
        private short[] shortBuffer;
        [FieldOffset(8)]
        private int[] intBuffer;


        /// <summary>
        /// Initializes a new instance of the <see cref="WaveBuffer"/> class binded to a specific byte buffer.
        /// </summary>
        /// <param name="bufferToBoundTo">A byte buffer to bound the WaveBuffer to.</param>
        public WaveBuffer(byte[] bufferToBoundTo)
        {
            BindTo(bufferToBoundTo);
        }

        /// <summary>
        /// Binds this WaveBuffer instance to a specific byte buffer.
        /// </summary>
        /// <param name="bufferToBoundTo">A byte buffer to bound the WaveBuffer to.</param>
        private void BindTo(byte[] bufferToBoundTo)
        {   
            /* WaveBuffer assumes the caller knows what they are doing. We will let this pass
             * if ( (bufferToBoundTo.Length % 4) != 0 )
            {
                throw new ArgumentException("The byte buffer to bound must be 4 bytes aligned");
            }*/
            byteBuffer = bufferToBoundTo;
            numberOfBytes = 0;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WaveBuffer"/> to <see cref="System.Byte"/>.
        /// </summary>
        /// <param name="waveBuffer">The wave buffer.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator byte[](WaveBuffer waveBuffer)
        {
            return waveBuffer.byteBuffer;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WaveBuffer"/> to <see cref="System.Single"/>.
        /// </summary>
        /// <param name="waveBuffer">The wave buffer.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator float[](WaveBuffer waveBuffer)
        {
            return waveBuffer.floatBuffer;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WaveBuffer"/> to <see cref="System.Int32"/>.
        /// </summary>
        /// <param name="waveBuffer">The wave buffer.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int[](WaveBuffer waveBuffer)
        {
            return waveBuffer.intBuffer;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="WaveBuffer"/> to <see cref="System.Int16"/>.
        /// </summary>
        /// <param name="waveBuffer">The wave buffer.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator short[](WaveBuffer waveBuffer)
        {
            return waveBuffer.shortBuffer;
        }

        public byte[] ByteBuffer => byteBuffer;

        public float[] FloatBuffer => floatBuffer;

        public short[] ShortBuffer => shortBuffer;

        public int[] IntBuffer => intBuffer;


        public int MaxSize => byteBuffer.Length;

        public int ByteBufferCount => numberOfBytes;

        public int FloatBufferCount => numberOfBytes / 4;


        public int ShortBufferCount => numberOfBytes / 2;

        public int IntBufferCount => numberOfBytes / 4;
    }
}
