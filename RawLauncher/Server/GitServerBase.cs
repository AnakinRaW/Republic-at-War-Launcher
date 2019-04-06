using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Server
{
    public abstract class GitServerBase : ErrorLoggingServer, IVersionServer
    {       
        public override string DownloadString(string resource)
        {
            string result;
            try
            {
                var webClient = new WebClient();
                var address = ServerRootAddress + resource;
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
            return UrlExists(string.Empty);
        }

        public override bool UrlExists(string resource)
        {
            var request = (HttpWebRequest)WebRequest.Create(ServerRootAddress + resource);
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
                var s = ServerRootAddress + resource;
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

        public abstract IEnumerable<ModVersion> GetAllVersions();
    }
}
