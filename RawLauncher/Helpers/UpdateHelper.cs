using System.Windows;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Helpers
{
    static class UpdateHelper
    {
        /// <summary>
        /// Throw a message asking for confirmation about resetting the Mod
        /// </summary>
        /// <returns>True if User wants to continue</returns>
        public static bool AskUserToContinue()
        {
            var result = MessageProvider.Show(MessageProvider.GetMessage("UpdateOperationQuestion"), "Republic at War",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            return result != MessageBoxResult.No;
        }
    }
}
