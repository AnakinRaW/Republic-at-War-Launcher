using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RawLauncherWPF.Utilities;
using static System.String;
using static RawLauncherWPF.NativeMethods.NativeMethods;

namespace RawLauncherWPF.Server
{
    public class HostServer : IHostServer
    {
        public HostServer(string address)
        {
            ServerRootAddress = address;
        }

        public bool CheckForUpdate(string currentVersion)
        {
            IsCheckingForUpdate = true;
            if (!IsRunning())
                MessageProvider.Show("Fail");
            else
            {
                MessageProvider.Show("No Fail");
                //TODO: Throw exception
            }

            IsCheckingForUpdate = false;
            return true;
        }

        public bool IsCheckingForUpdate { get; private set; }
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
                    MessageProvider.Show("Was not able to get data from: " + ServerRootAddress + resource);
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
            catch (Exception e)
            {
                if (ComputerHasInternetConnection())
                    MessageProvider.Show(e.Message +"\r\n Was not able to get data from: " + ServerRootAddress + resource);
            }
        }
    }
}