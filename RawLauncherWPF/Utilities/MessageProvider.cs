using System;
using System.Globalization;
using System.Resources;
using System.Windows;
using RawLauncherWPF.UI;

namespace RawLauncherWPF.Utilities
{
    public static class MessageProvider
    {
        private static ResourceManager _resourceManager = new ResourceManager("RawLauncherWPF.Localization.Res", typeof(MainWindow).Assembly);

        public static MessageBoxResult Show()
        {
            return MessageBox.Show("Test", string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }

        public static MessageBoxResult Show(string message)
        {
            return MessageBox.Show(message, "Republic at War", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }

        public static MessageBoxResult Show(string message, string caption)
        {
            return MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button)
        {
            return MessageBox.Show(message, caption, button, MessageBoxImage.None, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            return MessageBox.Show(message, caption, button, image, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button, MessageBoxImage image, MessageBoxResult result)
        {
            return MessageBox.Show(message, caption, button, image, result, MessageBoxOptions.DefaultDesktopOnly);
        }

        public static MessageBoxResult Show(string message, string caption, MessageBoxButton button,
            MessageBoxImage image, MessageBoxResult result, MessageBoxOptions options)
        {
            return MessageBox.Show(message, caption, button, image, result, options);
        }


        public static string GetMessage(string messageId, params object[] args)
        {
            try
            {
                var result = string.Format(_resourceManager.GetString(messageId, StartupLauncher.DisplayCulture), args);
                return result;
            }
            catch (Exception)
            {
                return _resourceManager.GetString("ErrorCreateMessageFailed");
            }
            
        }
    }
}
