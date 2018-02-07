using System.IO;
using RawLauncherWPF;

namespace RawLauncherTmp.Updaters
{
    internal class ThemeUpdater : AssemblyUpdater
    {
        public override string FileName => "RawLauncher.Theme.dll";
        
        public override string VersionsServerPath => @"master\AvailableLauncherThemeVersions.txt";

        protected override void Update()
        {
            var server = new UpdateServer(StartupLauncher.ServerUrl);
            if (!server.IsRunning())
                return;
            server.DownloadFile("master/LauncherUpdates/Themes/" + LatestVersion+  "/" + FileName, Path.Combine(Directory.GetCurrentDirectory(), FileName + ".new"));
            if (!File.Exists(FileName+ ".new"))
                return;
            DeleteCurrent();
            File.Move(FileName + ".new", FileName);
        }
    }
}
