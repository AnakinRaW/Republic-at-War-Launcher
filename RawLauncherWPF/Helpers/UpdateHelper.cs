using System;
using System.IO;
using System.Windows;
using static RawLauncherWPF.Utilities.MessageProvider;

namespace RawLauncherWPF.Helpers
{
    static class UpdateHelper
    {
        /// <summary>
        /// Throw a message asking for confirmation about resetting the Mod
        /// </summary>
        /// <returns>True if User wants to continue</returns>
        public static bool AskUserToContinue()
        {
            var result = Show(GetMessage("UpdateOperationQuestion"), "Republic at War",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            return result != MessageBoxResult.No;
        }
    }
}
