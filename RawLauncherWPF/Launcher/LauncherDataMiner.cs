using System;
using System.IO;
using RawLauncherWPF.Games;
using RawLauncherWPF.Mods;

namespace RawLauncherWPF.Launcher
{
    public class LauncherDataMiner
    {
        public const string EeawForum = "http://www.everythingeaw.com/forum/index.php";
        public const string GameconstantsUpdateHash = "4306d0c45d103cd11ff6743d1c3d9366";
        public const string GameconstantsUpdateHashEaW = "1d44b0797c8becbe240adc0098c2302a";
        public const string GraphicdetailsUpdateHash = "4d7e140887fc1dd52f47790a6e20b5c5";
        public const string ModdbPage = "http://www.moddb.com/mods/republic-at-war";
        public const string Server = "";
        public const string ServerUrl = "";
        public static readonly string CurrentDirectory = Directory.GetCurrentDirectory();

        public LauncherDataMiner()
        {
            DataMiner = this;
        }

        public static LauncherDataMiner DataMiner { get; private set; }
        public IMod CurrentMod { get; private set; }
        public IGame Eaw { get; private set; }
        public IGame Foc { get; private set; }
        public string RestoreDownloadDir { get; private set; }
        public string UpdateDownloadDir { get; private set; }

        public void SetCurrentMod(IMod mod)
        {
            if (mod == null)
                throw new NullReferenceException();
            CurrentMod = mod;
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
                throw new NullReferenceException();
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