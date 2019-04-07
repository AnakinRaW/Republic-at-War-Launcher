using System;
using System.Collections.Generic;
using System.IO;
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
            return UrlExists("running.txt");
        }

        public override bool UrlExists(string resource)
        {
            var request = (HttpWebRequest)WebRequest.Create(UrlCombine.Combine(ServerRootAddress, resource));
            request.Method = "HEAD";
            request.Timeout = 5000;
            try
            {
                request.GetResponse();
                request.Abort();
            }
            catch (WebException ex)
            {
                return ex.Response is HttpWebResponse response && (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound);
            }
            return true;
        }

        public void DownloadFile(string resource, string storagePath)
        {
            if (resource == null || storagePath == null)
                return;
            try
            {
                var webClient = new WebClient();
                var s = UrlCombine.Combine(ServerRootAddress, resource);
                if (!Directory.Exists(Path.GetDirectoryName(storagePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(storagePath));
                var uri = new Uri(s);
                webClient.DownloadFile(uri, storagePath);
            }
            catch (Exception)
            {
                if (NativeMethods.NativeMethods.ComputerHasInternetConnection())
                    MessageRecorder.AppandMessage(MessageProvider.GetMessage("ExceptionHostServerGetData", ServerRootAddress + resource));
            }
        }

        public string GetRescueFilePath(RescueFileType type, ModVersion version)
        {
            string fileName;
            switch (type)
            {
                case RescueFileType.Check:
                    fileName = Config.CheckFileFileName;
                    break;
                case RescueFileType.UpdateRestore:
                    fileName = Config.RestoreFileFileName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            return UrlCombine.Combine(version.ToFullString(), fileName);
        }

        public IEnumerable<ModVersion> GetAllVersions()
        {
            var data = DownloadString(ModVersionListRelativePath).ToStream();
            return VersionUtilities.SerializeVersionsToList(data);
        }
    }
}
