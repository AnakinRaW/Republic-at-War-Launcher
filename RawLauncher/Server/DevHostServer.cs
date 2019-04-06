using System;
using System.Collections.Generic;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Server
{
    public class DevHostServer : ErrorLoggingServer, IVersionServer
    {
        public static DevHostServer Instance { get; private set; }

        public override string ServerRootAddress { get; }

        private DevHostServer(string baseAddress)
        {
            Instance = this;
            ServerRootAddress = baseAddress;
        }

        public static void CreateInstance(string baseAddress)
        {
            if (Instance != null)
                return;
            new DevHostServer(baseAddress);
        }


        public override string DownloadString(string resource)
        {
            throw new NotImplementedException();
        }

        public override bool IsRunning()
        {
            throw new NotImplementedException();
        }

        public override bool UrlExists(string resource)
        {
            throw new NotImplementedException();
        }

        public void DownloadFile(string resource, string storagePath)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ModVersion> GetAllVersions()
        {
            return null;
        }
    }
}
