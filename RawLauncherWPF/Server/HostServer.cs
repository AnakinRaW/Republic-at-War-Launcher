using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using static System.String;

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
                MessageBox.Show("Fail");
            else
            {
                MessageBox.Show("No Fail");
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
                MessageBox.Show("Was not able to get data from: " + Path.GetFileName(ServerRootAddress + resource));
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
                return (response != null) && response.StatusCode == HttpStatusCode.Forbidden;
            }
            return true;
        }
    }
}