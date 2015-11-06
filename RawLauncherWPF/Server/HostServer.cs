using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using static System.String;
using static RawLauncherWPF.NativeMethods.NativeMethods;
using static RawLauncherWPF.Utilities.MessageProvider;

namespace RawLauncherWPF.Server
{
    public class HostServer : IHostServer
    {
        public HostServer(string address)
        {
            ServerRootAddress = address;
        }

        public async Task<bool> CheckRunningAsync() => await Task.FromResult(IsRunning());

        public string DownloadString(string resource)
        {
            string result;
            try
            {
                var webClient = new WebClient();
                result = webClient.DownloadString(ServerRootAddress + resource);
            }
            catch (Exception)
            {
                if (ComputerHasInternetConnection())
                    Show(GetMessage("ExceptionHostServerGetData", ServerRootAddress + resource));
                result = Empty;
            }
            return result;
        }

        public bool IsRunning() => UrlExists(Empty);
        public string ServerRootAddress { get; set; }

        public bool UrlExists(string resource)
        {
            var request = (HttpWebRequest) WebRequest.Create(ServerRootAddress + resource);
            request.Method = "HEAD";
            request.Timeout = 5000;
            try
            {
                request.GetResponse();
                request.Abort();
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                if ((response != null) && (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.BadRequest))
                    return true;
                return false;
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
                webClient.DownloadFile(new Uri(s), storagePath);
            }
            catch (Exception)
            {
                if (ComputerHasInternetConnection())
                    Show(GetMessage("ExceptionHostServerGetData", ServerRootAddress + resource));
            }
        }
    }
}