namespace RawLauncherWPF.Server
{
    public interface IHostServer : IServer
    {

        bool IsCheckingForUpdate { get; }

        /// <summary>
        /// Checks if there is an update for the current installed Mod
        /// </summary>
        /// <param name="currentVersion">Current Version of the mod</param>
        /// <returns>True if update avaiable, false if not</returns>
        bool CheckForUpdate(string currentVersion);

        void DownloadFile(string resource, string storagePath);
    }
}