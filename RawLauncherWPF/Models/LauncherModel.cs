using System;
using System.IO;
using RawLauncherWPF.Games;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Server;

namespace RawLauncherWPF.Models
{
    public class LauncherModel
    {
        public LauncherModel()
        {
            DataMiner = this;
        }

        public static LauncherModel DataMiner { get; private set; }
        public IMod CurrentMod { get; private set; }
        public IGame Eaw { get; private set; }
        public IGame Foc { get; private set; }
        public string RestoreDownloadDir { get; private set; }
        public string UpdateDownloadDir { get; private set; }
        public IHostServer HostServer { get; private set; }

        public bool QuietLaunchFileExists => File.Exists(Directory.GetCurrentDirectory() + @"\raw.txt");

        public void SetCurrentMod(IMod mod)
        {
            if (mod == null)
                throw new NullReferenceException();
            CurrentMod = mod;
        }

        public void SetHostServer(IHostServer server)
        {
            if (server == null)
                throw new NullReferenceException(nameof(server));
            HostServer = server;
        }

        public void SetEawGame(IGame game)
        {
            if (game == null)
                throw new NullReferenceException();
            Eaw = game;
        }

        public void SetFocGame(IGame game)
        {
            if (game == null)
                throw new NullReferenceException(nameof(game));
            Foc = game;
        }

        public void SetRestoreDownloadDir(string path)
        {
            if (path == null)
                throw new NullReferenceException(nameof(path));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            RestoreDownloadDir = path;
        }

        public void SetUpdateDownloadDir(string path)
        {
            if (path == null)
                throw new NullReferenceException(nameof(path));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            UpdateDownloadDir = path;
        }
    }
}