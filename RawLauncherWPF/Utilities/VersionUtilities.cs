using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RawLauncherWPF.Configuration;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.Server;
using RawLauncherWPF.ViewModels;

namespace RawLauncherWPF.Utilities
{
    public static class VersionUtilities
    {
        public static List<Version> GetAllAvailableModVersionsOnline()
        {
            var server = new HostServer(Config.ServerUrl);
            var data = server.DownloadString(Config.ModVersionListRelativePath).ToStream();  
            return SeriallizeVersionsToList(data);
        }

        public static Version GetLatestModVersion()
        {
            var versions = GetAllAvailableModVersionsOnline();
            if (versions == null || versions.Count == 0)
                versions = GetAllAvailableModVersionsOffline();

            return versions != null ? versions.Last() : new Version("0.1");
        }


        public static Version GetLatestThemeVersion()
        {
            var versions = GetAllAvailableThemeVersionsOnline();
            if (versions == null || versions.Count == 0)
                return null;
            return versions.Last();
        }

        private static List<Version> GetAllAvailableThemeVersionsOnline()
        {
            var server = new HostServer(Config.ServerUrl);
            var data = server.DownloadString(Config.ThemeVersionListRelativePath).ToStream();
            return SeriallizeVersionsToList(data);
        }


        public static List<Version> GetAllAvailableModVersionsOffline()
        {
            if (!Directory.Exists(LauncherViewModel.RestoreDownloadDirStatic) ||
                !File.Exists(LauncherViewModel.RestoreDownloadDirStatic + Config.ModVersionListRelativePath))
                return SeriallizeVersionsToList(Stream.Null);
            var data =
                FileUtilities.FileToStream(LauncherViewModel.RestoreDownloadDirStatic + Config.ModVersionListRelativePath);
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
