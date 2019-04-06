using System;
using System.Collections.Generic;
using System.Net;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.ExtensionClasses;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Server
{
    public class DevHostServer : ErrorLoggingServer, IVersionServer
    {
        protected string ModVersionListRelativePath = $"{Config.AvailableModVersionsFileName}";

        public static DevHostServer Instance { get; private set; }

        public override string ServerRootAddress { get; }

        private DevHostServer(string baseAddress)
        {
            Instance = this;
            ServerRootAddress = "https://republicatwar.com/downloads/patches/";
        }

        public static void CreateInstance(string baseAddress)
        {
            if (Instance != null)
                return;
            new DevHostServer(baseAddress);
        }


        public override string DownloadString(string resource)
        {
            string result;
            try
            {
                var webClient = new WebClient();
                var address = UrlCombine.Combine(ServerRootAddress, resource);
                var uri = new Uri(address, UriKind.Absolute);
                result = webClient.DownloadString(uri);
                //result = webClient.DownloadString(ServerRootAddress + resource);
            }
            catch (Exception)
            {
                if (NativeMethods.NativeMethods.ComputerHasInternetConnection())
                    MessageRecorder.AppandMessage(MessageProvider.GetMessage("ExceptionHostServerGetData", ServerRootAddress + resource));
                result = string.Empty;
            }
            return result;
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
            var data = DownloadString(ModVersionListRelativePath).ToStream();
            return VersionUtilities.SerializeVersionsToList(data);
        }
    }
}
