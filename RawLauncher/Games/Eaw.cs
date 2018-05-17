using System;
using System.IO;
using Microsoft.Win32;
using RawLauncher.Framework.Hash;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Games
{
    public class Eaw : IGame
    {
        public const string GameconstantsUpdateHashEaW = "1d44b0797c8becbe240adc0098c2302a";

        public void DeleteMod(IMod mod)
        {
            throw new NotImplementedException();
        }

        public void ClearDataFolder()
        {
            //Ignored
        }

        public bool IsGameAiClear()
        {
            throw new NotImplementedException();
        }

        public void BackUpAiFiles()
        {
            throw new NotImplementedException();
        }

        public void ResotreAiFiles()
        {
            throw new NotImplementedException();
        }

        public void DeleteMod(string name)
        {
            //Ignored
        }

        public GameProcessData GameProcessData => new GameProcessData();
        public bool Exists() => Directory.Exists(GameDirectory) && File.Exists(Path.Combine(GameDirectory, "sweaw.exe"));
        public string GameDirectory { get; protected set; }

        public Eaw(string gameDirectory)
        {
            GameDirectory = gameDirectory;
            if (!Exists())
                throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExist"));
        }

        public bool IsPatched()
        {
            if (!File.Exists(Path.Combine(GameDirectory, @"Data\XML\GAMECONSTANTS.xml")))
                return false;
            var hashProvider = new HashProvider();
            if (hashProvider.GetFileHash(Path.Combine(GameDirectory, @"Data\XML\GAMECONSTANTS.xml")) != GameconstantsUpdateHashEaW)
                return false;
            if (Directory.GetFiles(Path.Combine(GameDirectory, @"Data\XML\")).Length != 1)
                return false;
            return true;
        }

        public string Name => "Empire at War";

        public bool Patch()
        {
            try
            {
                if (Directory.Exists(Path.Combine(GameDirectory, @"Data\XML\")))
                    Directory.Delete(Path.Combine(GameDirectory, @"Data\XML\"), true);
                Directory.CreateDirectory(Path.Combine(GameDirectory, @"Data\XML\"));
                File.WriteAllText(Path.Combine(GameDirectory, @"Data\XML\GAMECONSTANTS.xml"), Properties.Resources.GAMECONSTANTSeaw);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void PlayGame()
        {
            //Ignored
        }

        public void PlayGame(IMod mod)
        {
            //Ignored
        }

        public string SaveGameDirectory => throw new NotImplementedException();

        public static bool FindInstallationRelativeToFoc(string focPath, GameType type, out string eawPath)
        {
            eawPath = string.Empty;

            switch (type)
            {
                case GameType.Disk:
                    break;
                case GameType.SteamGold:
                    break;
                case GameType.GoG:
                    break;
                case GameType.DiskGold:
                case GameType.Undefined:
                    return false;
            }

            return true;
        }
    }
}