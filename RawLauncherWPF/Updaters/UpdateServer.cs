using System;
using System.IO;
using System.Net;

namespace RawLauncherWPF.Updaters
{
    public class UpdateServer
    {
        public string ServerRootAddress { get; set; }

        public UpdateServer(string baseAdress)
        {
            ServerRootAddress = baseAdress;
        }

        public string DownloadString(string resource)
        {
            string result;
            try
            {
                var webClient = new WebClient();

                var url = ServerRootAddress + resource;

                result = webClient.DownloadString(url);
            }
            catch (Exception)
            {
                result = string.Empty;
            }
            return result;
        }

        public bool IsRunning() => UrlExists(string.Empty);


        public bool UrlExists(string resource)
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
                var response = ex.Response as HttpWebResponse;
                if (response != null && (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound))
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
                var uri = new Uri(s);
                webClient.DownloadFile(uri, storagePath);
            }
            catch (Exception)
            {
                //Ignored
            }
        }

    }
}
