using System.Collections.Generic;

namespace RawLauncherWPF.Server
{
    public interface IServer
    {
        /// <summary>
        /// The Root Address of the Server.
        /// </summary>
        string ServerRootAddress { get; set; }

        /// <summary>
        /// Checks if Server is running (403 means also running)
        /// </summary>
        /// <returns>True if running, false if not</returns>
        bool IsRunning();

        /// <summary>
        /// Checks if a Url based onto the ServerRootAddress is running
        /// </summary>
        /// <param name="resource">Full resource path</param>
        /// <returns>True if found, false if not</returns>
        bool UrlExists(string resource);
    }
}