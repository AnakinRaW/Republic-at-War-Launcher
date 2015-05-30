using System;
using System.IO;
using RawLauncherWPF.Games;
using RawLauncherWPF.Mods;

namespace RawLauncherWPF.Launcher
{
    public class LauncherDataMiner
    {
        public const string GameconstantsUpdateHashEaW = "1d44b0797c8becbe240adc0098c2302a";
        public const string GameconstantsUpdateHash = "4306d0c45d103cd11ff6743d1c3d9366";
        public const string GraphicdetailsUpdateHash = "4d7e140887fc1dd52f47790a6e20b5c5";
        public const string Server = "";
        public const string ServerUrl = "";
        public const string EeawForum = "http://www.everythingeaw.com/forum/index.php";
        public const string ModdbPage = "http://www.moddb.com/mods/republic-at-war";

        public static readonly string CurrentDirectory = Directory.GetCurrentDirectory();
        public static LauncherDataMiner DataMiner { get; private set; }

        public static Game Foc { get; private set; }

        public static Game Eaw { get; private set; }
        public static Mod CurrentMod { get; private set; }

        public static string UpdateDownloadDir { get; private set; }

        public static string RestoreDownloadDir { get; private set; }

        public LauncherDataMiner()
        {
            DataMiner = this;
        }

        public void SetCurrentMod(Mod mod)
        {
            if (mod == null)
                throw new NullReferenceException();
            CurrentMod = mod;
        }

        public void SetFocGame(Game game)
        {
            if (game == null)
                throw new NullReferenceException();
            Foc = game;
        }

        public void SetEawGame(Game game)
        {
            if (game == null)
                throw new NullReferenceException();
            Eaw = game;
        }

        public void SetUpdateDownloadDir(string path)
        {
            if (path == null)
                throw new NullReferenceException(nameof(path));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            UpdateDownloadDir = path;
        }

        public void SetRestoreDownloadDir(string path)
        {
            if (path == null)
                throw new NullReferenceException(nameof(path));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            RestoreDownloadDir = path;
        }
    }
}
