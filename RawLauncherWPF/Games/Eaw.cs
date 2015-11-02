using System;
using System.IO;
using RawLauncherWPF.Hash;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Properties;

namespace RawLauncherWPF.Games
{
    public class Eaw : IGame
    {
        public Eaw()
        {
        }

        public Eaw(string gameDirectory)
        {
            GameDirectory = gameDirectory;
            if (!Exists())
                throw new GameExceptions("This Game does not exists");
        }

        public string GameDirectory { get; }
        public string Name => "Empire at War";

        public string SaveGameDirectory
        {
            get { throw new NotImplementedException();}
        }

        public void ClearDataFolder()
        {
            //Ignored
        }

        public void DeleteMod(string name)
        {
            //Ignored
        }

        public bool Exists() => Directory.Exists(GameDirectory);

        public IGame FindGame()
        {
            if (!File.Exists(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Star Wars Empire at War\GameData\sweaw.exe"))
                throw new GameExceptions(Name + " does not exists");
            return new Eaw(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\Star Wars Empire at War\GameData\");
        }

        public bool IsPatched()
        {
            if (!File.Exists(GameDirectory + @"Data\XML\GAMECONSTANTS.xml"))
                return false;
            var hashProvider = new HashProvider();
            if (hashProvider.GetFileHash(GameDirectory + @"Data\XML\GAMECONSTANTS.xml") !=
                Configuration.Config.GameconstantsUpdateHashEaW)
                return false;
            if (Directory.GetFiles(GameDirectory + @"Data\XML").Length != 1)
                return false;
            return true;
        }

        public bool Patch()
        {
            try
            {
                if (Directory.Exists(GameDirectory + @"Data\XML"))
                    Directory.Delete(GameDirectory + @"Data\XML", true);
                Directory.CreateDirectory(GameDirectory + @"Data\XML");
                File.WriteAllText(GameDirectory + @"Data\XML\GAMECONSTANTS.XML", Resources.GAMECONSTANTSeaw);
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
    }
}
