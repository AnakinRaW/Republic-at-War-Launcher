using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using RawLauncherWPF.Configuration;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.ViewModels;
using static RawLauncherWPF.Utilities.MessageProvider;

namespace RawLauncherWPF.Utilities
{
    public static class VersionUtilities
    {
        public static List<Version> GetAllAvailableVersionsOnline()
        {
            var data = LauncherViewModel.HostServerStatic.DownloadString(Config.VersionListRelativePath).ToStream();  
            return SeriallizeVersionsToList(data);
        }

        public static Version GetLatestVersion()
        {
            var versions = GetAllAvailableVersionsOnline();
            if (versions == null || versions.Count == 0)
                versions = GetAllAvailableVersionsOffline();

            return versions != null ? versions.Last() : new Version("0.1");
        }

        public static bool AskToUpdate()
        {
            var result =
                        Show(GetMessage("VersionUtilitiesAskForUpdate", GetLatestVersion()),
                            "Republic at War", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            return result == MessageBoxResult.Yes;
        }

        public static List<Version> GetAllAvailableVersionsOffline()
        {
            if (!Directory.Exists(LauncherViewModel.RestoreDownloadDirStatic) ||
                !File.Exists(LauncherViewModel.RestoreDownloadDirStatic + Config.VersionListRelativePath))
                return SeriallizeVersionsToList(Stream.Null);
            var data =
                FileUtilities.FileToStream(LauncherViewModel.RestoreDownloadDirStatic + Config.VersionListRelativePath);
            return SeriallizeVersionsToList(data);
        }

        private static List<Version> SeriallizeVersionsToList(Stream dataStream)
        {
            var list = new List<Version>();
            var reader = new StreamReader(dataStream);
            while (!reader.EndOfStream)
                list.Add(new Version(reader.ReadLine()));
            return list;
        } 
    }
}
