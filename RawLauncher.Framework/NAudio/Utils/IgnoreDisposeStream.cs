using System.IO;

namespace RawLauncher.Framework.NAudio.Utils
{
    public class IgnoreDisposeStream : Stream
    {
        /// <summary>
        /// The source stream all other methods fall through to
        /// </summary>
        private Stream SourceStream { get; set; }

        /// <summary>
        /// If true the Dispose will be ignored, if false, will pass through to the SourceStream
        /// Set to true by default
        /// </summary>
        private bool IgnoreDispose { get; }

        /// <inheritdoc />
        /// <summary>
        /// Creates a new IgnoreDisposeStream
        /// </summary>
        /// <param name="sourceStream">The source stream</param>
        public IgnoreDisposeStream(Stream sourceStream)
        {
            SourceStream = sourceStream;
            IgnoreDispose = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Can Read
        /// </summary>
        public override bool CanRead => SourceStream.CanRead;

        /// <inheritdoc />
        /// <summary>
        /// Can Seek
        /// </summary>
        public override bool CanSeek => SourceStream.CanSeek;

        /// <inheritdoc />
        /// <summary>
        /// Can write to the underlying stream
        /// </summary>
        public override bool CanWrite => SourceStream.CanWrite;

        /// <inheritdoc />
        /// <summary>
        /// Flushes the underlying stream
        /// </summary>
        public override void Flush()
        {
            SourceStream.Flush();
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the length of the underlying stream
        /// </summary>
        public override long Length => SourceStream.Length;

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the position of the underlying stream
        /// </summary>
        public override long Position
        {
            get => SourceStream.Position;
            set => SourceStream.Position = value;
        }

        /// <inheritdoc />
        /// <summary>
        /// Reads from the underlying stream
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return SourceStream.Read(buffer, offset, count);
        }

        /// <inheritdoc />
        /// <summary>
        /// Seeks on the underlying stream
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return SourceStream.Seek(offset, origin);
        }

        /// <inheritdoc />
        /// <summary>
        /// Sets the length of the underlying stream
        /// </summary>
        public override void SetLength(long value)
        {
            SourceStream.SetLength(value);
        }

        /// <inheritdoc />
        /// <summary>
        /// Writes to the underlying stream
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            SourceStream.Write(buffer, offset, count);
        }

        /// <inheritdoc />
        /// <summary>
        /// Dispose - by default (IgnoreDispose = true) will do nothing,
        /// leaving the underlying stream undisposed
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (!IgnoreDispose)
            {
                SourceStream.Dispose();
                SourceStream = null;
            }
        }
    }
}
