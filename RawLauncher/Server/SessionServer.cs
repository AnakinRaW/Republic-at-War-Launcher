using System;
using System.Net;
using System.Threading.Tasks;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Server
{
    public class SessionServer : IServer
    {
        public SessionServer(string address)
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
                MessageProvider.Show(MessageProvider.GetMessage("ExceptionHostServerGetData", ServerRootAddress + resource));
                result = string.Empty;
            }
            return result;
        }

        public bool IsRunning() => UrlExists(string.Empty);
        public string ServerRootAddress { get; set; }

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
                return ex.Response is HttpWebResponse response && response.StatusCode == HttpStatusCode.Forbidden;
            }
            return true;
        }
    }
}