namespace RawLauncher.Framework.Server
{
    public interface IHostServer : IServer
    {

        bool HasErrors { get; }

        /// <summary>
        /// Downloads a file sync and save it to the storage Path. If Storage Path does not exists it will be created
        /// </summary>
        /// <param name="resource">Relative path to resource</param>
        /// <param name="storagePath">Absolute file path</param>
        void DownloadFile(string resource, string storagePath);

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