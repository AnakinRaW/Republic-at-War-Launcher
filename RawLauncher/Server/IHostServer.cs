using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Server
{
    public interface IHostServer : IServer
    {
        /// <summary>
        /// Downloads a file sync and save it to the storage Path. If Storage Path does not exists it will be created
        /// </summary>
        /// <param name="resource">Relative path to resource</param>
        /// <param name="storagePath">Absolute file path</param>
        void DownloadFile(string resource, string storagePath);

        string GetRescueFilePath(RescueFileType type, ModVersion version);
    }

    public enum RescueFileType
    {
        Check, 
        UpdateRestore
    }
}