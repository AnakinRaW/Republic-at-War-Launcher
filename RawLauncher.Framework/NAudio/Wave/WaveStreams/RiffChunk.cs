
namespace RawLauncher.Framework.NAudio.Wave.WaveStreams
{
    /// <summary>
    /// Holds information about a RIFF file chunk
    /// </summary>
    public class RiffChunk
    {
        /// <summary>
        /// Creates a RiffChunk object
        /// </summary>
        public RiffChunk(int length, long streamPosition)
        {
            Length = length;
            StreamPosition = streamPosition;
        }

        /// <summary>
        /// The chunk length
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// The stream position this chunk is located at
        /// </summary>
        public long StreamPosition { get; }
    }
}
