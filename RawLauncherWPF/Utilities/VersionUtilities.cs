using System;
using System.Collections.Generic;
using System.IO;
using RawLauncherWPF.Configuration;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.Utilities
{
    public static class VersionUtilities
    {
        public static List<Version> GetAllAvailableVersionsOnline()
        {
            var data = LauncherViewModel.HostServerStatic.DownloadString(Config.VersionListRelativePath).ToStream();  
            return SeriallizeVersionsToList(data);
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
