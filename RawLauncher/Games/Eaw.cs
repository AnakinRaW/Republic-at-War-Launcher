using System;
using System.IO;
using System.Windows;
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
        public bool Exists() => Directory.Exists(GameDirectory);

        public IGame FindGame()
        {
            if (File.Exists(Directory.GetParent(Directory.GetCurrentDirectory()) +
                            @"\Star Wars Empire at War\GameData\sweaw.exe"))
            {
                GameDirectory = Directory.GetParent(Directory.GetCurrentDirectory()) +
                                @"\Star Wars Empire at War\GameData\";
                return this;
            }

            //Try by registry
            var exePath = (string) Registry.LocalMachine
                .OpenSubKey(@"SOFTWARE\LucasArts\Star Wars Empire at War\1.0", false)?.GetValue("ExePath");
            if (exePath != null && File.Exists(exePath))
            {
                GameDirectory = new FileInfo(exePath).Directory.FullName;
                return this;
            }
            throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExistName", Name));
        }

        public string GameDirectory { get; protected set; }

        public bool IsPatched()
        {
            if (!File.Exists(GameDirectory + @"Data\XML\GAMECONSTANTS.xml"))
                return false;
            var hashProvider = new HashProvider();
            if (hashProvider.GetFileHash(GameDirectory + @"Data\XML\GAMECONSTANTS.xml") != GameconstantsUpdateHashEaW)
                return false;
            if (Directory.GetFiles(GameDirectory + @"Data\XML").Length != 1)
                return false;
            return true;
        }

        public string Name => "Empire at War";

        public bool Patch()
        {
            try
            {
                if (Directory.Exists(GameDirectory + @"Data\XML"))
                    Directory.Delete(GameDirectory + @"Data\XML", true);
                Directory.CreateDirectory(GameDirectory + @"Data\XML");
                File.WriteAllText(GameDirectory + @"Data\XML\GAMECONSTANTS.XML", Properties.Resources.GAMECONSTANTSeaw);
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
    }
}