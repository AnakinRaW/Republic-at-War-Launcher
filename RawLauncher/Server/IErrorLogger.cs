namespace RawLauncher.Framework.Server
{
    public interface IErrorLogger
    {
        bool HasErrors { get; }

        /// <summary>
        /// Flushes the error log
        /// </summary>
        void FlushErrorLog();

        /// <summary>
        /// shows the current error log
        /// </summary>
        void ShowLog();
    }
}