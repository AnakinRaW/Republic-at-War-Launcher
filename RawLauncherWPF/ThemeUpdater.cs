using System;
using System.Diagnostics;
using System.IO;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.Server;

namespace RawLauncherWPF
{  
    public abstract class AssemblyUpdater
    {
        private Version _latestVerion;
        public abstract string FileName { get; }
        
        public abstract string VersionsServerPath { get; }

        protected Version LatestVersion =>
            _latestVerion ?? (_latestVerion = VersionUtilities.GetLatestFileVersion(VersionsServerPath));

        protected Version CurrentVersion
        {
            get
            {
                if (!File.Exists(FileName))
                    throw new FileNotFoundException(FileName);
                return new Version(FileVersionInfo.GetVersionInfo(FileName).FileVersion);
            }
        }

        public void UpdateIfNewVersionExists()
        {
            if (LatestVersion == null)
                return;

            if (CurrentVersion < LatestVersion)
                Update();
        }

        protected abstract void Update();

        protected void DeleteCurrent()
        {
            if (File.Exists(FileName))
                File.Delete(FileName);
        }
    }
    internal class FrameworkUpdater : AssemblyUpdater
    {
        public override string FileName => "RawLauncher.Framework.dll";

        public override string VersionsServerPath => @"\master\AvailableLauncherVersions.txt";

        protected override void Update()
        {
            var server = new HostServer(Config.ServerUrl);
            if (!server.IsRunning())
                return;
            server.DownloadFile(LatestVersion + "/Themes/" + FileName, Path.Combine(Directory.GetCurrentDirectory(), FileName + ".new"));
            if (!File.Exists(FileName + ".new"))
                return;
            DeleteCurrent();
            File.Move(FileName + ".new", FileName);
        }
    }
    internal class ThemeUpdater : AssemblyUpdater
    {
        public override string FileName => "RawLauncher.Theme.dll";
        
        public override string VersionsServerPath => @"\master\AvailableLauncherThemeVersions.txt";

        protected override void Update()
        {
            var server = new HostServer(Config.ServerUrl);
            if (!server.IsRunning())
                return;
            server.DownloadFile(LatestVersion + "/Themes/" + FileName, Path.Combine(Directory.GetCurrentDirectory(), FileName + ".new"));
            if (!File.Exists(FileName+ ".new"))
                return;
            DeleteCurrent();
            File.Move(FileName + ".new", FileName);
        }
    }
}
