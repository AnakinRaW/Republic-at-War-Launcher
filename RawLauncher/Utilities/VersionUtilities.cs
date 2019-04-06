using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Server;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Utilities
{
    public static class VersionUtilities
    {
        public static List<ModVersion> GetAllAvailableModVersionsOnline()
        {
            var launcher = IoC.Get<LauncherModel>();
            var servers = IoC.GetAll<IVersionServer>().ToList();
            if (launcher.UseDevHostServer)
                servers.Add(DevHostServer.Instance);

            var versions = new List<ModVersion>();
            foreach (var versionServer in servers)
                versions.AddRange(versionServer.GetAllVersions());

            SaveVersionsToDisk(versions);
            return versions;
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
            var path = Path.Combine(launcher.RestoreDownloadDir, Config.AvailableModVersionsFileName);

            if (!Directory.Exists(launcher.RestoreDownloadDir) || !File.Exists(path))
                return SerializeVersionsToList(Stream.Null);
            var data = FileUtilities.FileToStream(path);
            return SerializeVersionsToList(data);
        }

        internal static List<ModVersion> SerializeVersionsToList(Stream dataStream)
        {
            var list = new List<ModVersion>();
            var reader = new StreamReader(dataStream);
            while (!reader.EndOfStream)
                list.Add(ModVersion.Parse(reader.ReadLine()));
            return list;
        }

        private static void SaveVersionsToDisk(IEnumerable<ModVersion> versions)
        {
            var versionNames = versions.Select(modVersion => modVersion.ToFullString()).ToList();
            var launcher = IoC.Get<LauncherModel>();
            try
            {
                File.WriteAllLines(Path.Combine(launcher.RestoreDownloadDir, Config.AvailableModVersionsFileName), versionNames);
            }
            catch (IOException)
            {
            }
        }
    }
}
