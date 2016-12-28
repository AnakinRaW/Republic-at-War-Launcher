using System;
using System.Diagnostics;
using System.IO;
using RawLauncherWPF.Configuration;
using RawLauncherWPF.Server;

namespace RawLauncherWPF.Utilities
{
    internal class ThemeUpdater
    {

        private const string FileName = "RawLauncher.Theme.dll";


        public ThemeUpdater()
        {
            LatestVersion = VersionUtilities.GetLatestThemeVersion();
        }

        public Version LatestVersion { get; } 


        public void UpdateIfNewVersionExists()
        {
            if (LatestVersion == null)
                return;

            if (CurrentVersion < LatestVersion)
                UpdateTheme();
        }

        public Version CurrentVersion
        {
            get
            { 
                if (!File.Exists(FileName))
                    throw new FileNotFoundException(FileName);
                return new Version(FileVersionInfo.GetVersionInfo(FileName).FileVersion);
            }
        }

        public void UpdateTheme()
        {
            var server = new HostServer(Config.ServerUrl);
            if (!server.IsRunning())
                return;
            DeleteCurrentTheme();
            server.DownloadFile("Themes/" + LatestVersion + "/" + FileName, Path.Combine(Directory.GetCurrentDirectory(), FileName));
        }

        public void DeleteCurrentTheme()
        {
            if (File.Exists(FileName))
                File.Delete(FileName);            
        }

    }
}
