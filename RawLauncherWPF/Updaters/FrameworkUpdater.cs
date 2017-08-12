using System.IO;

namespace RawLauncherWPF.Updaters
{
    internal class FrameworkUpdater : AssemblyUpdater
    {
        public override string FileName => "RawLauncher.Framework.dll";

        public override string VersionsServerPath => @"\master\AvailableLauncherVersions.txt";

        protected override void Update()
        {
            var server = new UpdateServer(StartupLauncher.ServerUrl);
            if (!server.IsRunning())
                return;
            server.DownloadFile("/master/LauncherUpdates/Framework/" + LatestVersion + "/" + FileName, Path.Combine(Directory.GetCurrentDirectory(), FileName + ".new"));
            if (!File.Exists(FileName + ".new"))
                return;
            DeleteCurrent();
            File.Move(FileName + ".new", FileName);
        }
    }
}