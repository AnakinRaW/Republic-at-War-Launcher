using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.ExtensionClasses;
using RawLauncher.Framework.Server;
using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.Utilities
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
