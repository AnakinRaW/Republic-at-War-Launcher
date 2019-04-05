using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.ExtensionClasses;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Server;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Utilities
{
    public static class VersionUtilities
    {
        public static List<ModVersion> GetAllAvailableModVersionsOnline()
        {
            var server = IoC.Get<IHostServer>();
            var data = server.DownloadString(Config.ModVersionListRelativePath).ToStream();  
            return SeriallizeVersionsToList(data);
        }

        public static ModVersion GetLatestModVersion()
        {
            var versions = GetAllAvailableModVersionsOnline();
            if (versions == null || versions.Count == 0)
                versions = GetAllAvailableModVersionsOffline();

            return versions != null ? versions.Last() : ModVersion.Parse("0.1");
        }

        public static List<ModVersion> GetAllAvailableModVersionsOffline()
        {
            var launcher = IoC.Get<LauncherModel>();
            if (!Directory.Exists(launcher.RestoreDownloadDir) ||
                !File.Exists(launcher.RestoreDownloadDir + Config.ModVersionListRelativePath))
                return SeriallizeVersionsToList(Stream.Null);
            var data =
                FileUtilities.FileToStream(launcher.RestoreDownloadDir + Config.ModVersionListRelativePath);
            return SeriallizeVersionsToList(data);
        }

        private static List<ModVersion> SeriallizeVersionsToList(Stream dataStream)
        {
            var list = new List<ModVersion>();
            var reader = new StreamReader(dataStream);
            while (!reader.EndOfStream)
                list.Add(ModVersion.Parse(reader.ReadLine()));
            return list;
        } 
    }
}
