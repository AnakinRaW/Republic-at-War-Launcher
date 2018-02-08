using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.ExtensionClasses;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Server;

namespace RawLauncher.Framework.Utilities
{
    public static class VersionUtilities
    {
        public static List<Version> GetAllAvailableModVersionsOnline()
        {
            var server = IoC.Get<IHostServer>();
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
            var launcher = IoC.Get<LauncherModel>();
            if (!Directory.Exists(launcher.RestoreDownloadDir) ||
                !File.Exists(launcher.RestoreDownloadDir + Config.ModVersionListRelativePath))
                return SeriallizeVersionsToList(Stream.Null);
            var data =
                FileUtilities.FileToStream(launcher.RestoreDownloadDir + Config.ModVersionListRelativePath);
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
