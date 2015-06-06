using System;

namespace RawLauncherWPF.Server
{
    public class HostServer : IHostServer
    {
        public HostServer(string address)
        {
            ServerRootAddress = address;
        }

        public string ServerRootAddress { get; set; }
        public bool IsRunning()
        {
            return true;
        }

        public bool UrlExists(string resource)
        {
            return true;
        }

        public bool CheckForUpdate(string currentVersion)
        {
            return true;
        }
    }
}
