﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using ModernApplicationFramework.Controls;
using RawLauncherWPF.ViewModels;
using static RawLauncherWPF.Utilities.MessageProvider;
using static RawLauncherWPF.Utilities.VersionUtilities;

namespace RawLauncherWPF.Helpers
{
    public static class RestoreHelper
    {
        public static List<ComboBoxItem> CreateVersionItems()
        {
            var versions = GetAllAvailableVersionsOnline();
            return (from version in versions
                where version <= LauncherViewModel.CurrentModStatic.Version
                select new ComboBoxItem {Content = version, DataContext = version}).ToList();
        }

        /// <summary>
        /// Throw a message asking for confirmation about resetting the Mod
        /// </summary>
        /// <returns>True if User wants to continue</returns>
        public static bool AskUserToContinue()
        {
            var result = Show("Are you sure you want to restore Republic at War ?\r\n" + "This cannot be undone\r\n" +
                              "Modified Files will be delted and restored with the original ones", "Republic at War",
                MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);
            return result != MessageBoxResult.No;
        }

        public static bool IgnoreFile(string file)
        {
            if (file == null)
                throw new NullReferenceException(nameof(file));
            if (Path.GetFullPath(file).Contains("\\Text\\"))
                return true;
            if (Path.GetFullPath(file).Contains("\\Audio\\Speech\\"))
                return true;
            var s = new FileInfo(file).Directory?.Name;
            if (s == "Audio")
                return true;
            if (Path.GetFileName(file).Contains("Speech.meg"))
                return true;
            return false;
        }
    }
}
