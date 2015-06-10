using System;
using System.Net;
using System.Windows;

namespace RawLauncherWPF.Server
{
    public class HostServer : IHostServer
    {
        public HostServer(string address)
        {
            ServerRootAddress = address;
        }

        public string ServerRootAddress { get; set; }
        public bool IsRunning() => UrlExists(string.Empty);

        public bool UrlExists(string resource)
        {
            var request = (HttpWebRequest) WebRequest.Create(ServerRootAddress + resource);
            request.Method = "HEAD";
            request.Timeout = 3000;
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

        public bool IsCheckingForUpdate { get; private set; }

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
    }
}
