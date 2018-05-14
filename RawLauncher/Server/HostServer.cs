using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Server
{
    [Export(typeof(IHostServer))]
    public class HostServer : IHostServer
    {
        private readonly MessageRecorder _messageRecorder;

        public HostServer()
        {
            ServerRootAddress = Config.ServerUrl;
            _messageRecorder = new MessageRecorder();
        }

        public void FlushErrorLog() => _messageRecorder.Flush();

        public void ShowLog()
        {
            if (!HasErrors)
                return;

            var result = MessageProvider.Show(MessageProvider.GetMessage("DownloadFailedQuestion"), "Republic at War", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
                _messageRecorder.Save(MessageProvider.GetMessage("DownloadFailed"));
        }

        public async Task<bool> CheckRunningAsync() => await Task.FromResult(IsRunning());

        public string DownloadString(string resource)
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
                    _messageRecorder.AppandMessage(MessageProvider.GetMessage("ExceptionHostServerGetData", ServerRootAddress + resource));
                result = String.Empty;
            }
            return result;
        }

        public bool IsRunning() => UrlExists(String.Empty);
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
                return ex.Response is HttpWebResponse response && (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound);
            }
            return true;
        }

        public bool HasErrors => _messageRecorder.Count() > 0;

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
                    _messageRecorder.AppandMessage(MessageProvider.GetMessage("ExceptionHostServerGetData", ServerRootAddress + resource));
            }
        }
    }
}