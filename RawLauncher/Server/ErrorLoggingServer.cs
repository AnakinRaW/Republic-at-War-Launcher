using System.Windows;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Server
{
    public abstract class ErrorLoggingServer : IServer, IErrorLogger
    {
        protected readonly MessageRecorder MessageRecorder;

        public abstract string ServerRootAddress { get; }

        protected ErrorLoggingServer()
        {
            MessageRecorder = new MessageRecorder();
        }

        public abstract string DownloadString(string resource);

        public abstract bool IsRunning();

        public abstract bool UrlExists(string resource);

        public bool HasErrors => MessageRecorder.Count() > 0;

        public void FlushErrorLog()
        {
            MessageRecorder.Flush();
        }

        public void ShowLog()
        {
            if (!HasErrors)
                return;

            var result = MessageProvider.Show(MessageProvider.GetMessage("DownloadFailedQuestion"), "Republic at War", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
                MessageRecorder.Save(MessageProvider.GetMessage("DownloadFailed"));
        }
    }
}