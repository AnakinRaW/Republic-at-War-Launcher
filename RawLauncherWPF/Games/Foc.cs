using System;
using System.Diagnostics;
using System.IO;
using RawLauncherWPF.Hash;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Properties;
using RawLauncherWPF.Utilities;

namespace RawLauncherWPF.Games
{
    public class Foc : IGame
    {
        public Foc()
        {
        }

        public Foc(string gameDirectory)
        {
            GameDirectory = gameDirectory;
            if (!Exists())
                throw new GameExceptions("This Game does not exists");
        }

        public string GameDirectory { get; }

        public string Name => "Forces of Corruption";

        public bool Exists() => File.Exists(GameDirectory + @"\swfoc.exe");

        public IGame FindGame()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\swfoc.exe"))
                throw new GameExceptions(Name + " does not exists");
            return new Foc(Directory.GetCurrentDirectory() + @"\");
        }

        public void PlayGame()
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = GameDirectory + @"\swfoc.exe",
                    WorkingDirectory = GameDirectory,
                    UseShellExecute = false
                }
            };
            try
            {
                process.Start();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public void PlayGame(IMod mod)
        {
            if (!mod.Exists())
                throw new ModExceptions("Mod does not exists anymore.");
            if (!mod.ModDirectory.StartsWith(GameDirectory))
                throw new ModExceptions("Mod is not compatible");
            if (!mod.HasCorrectFolderStructure)
                throw new ModExceptions("Mod is not correct installed");

            var process = new Process
            {
                StartInfo =
                {
                    FileName = GameDirectory + @"\swfoc.exe",
                    Arguments = "MODPATH=" + mod.LaunchArgumentPath,
                    WorkingDirectory = GameDirectory,
                    UseShellExecute = false
                }
            };
            try
            {
                process.Start();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public bool Patch()
        {
            try
            {
                if (!Directory.Exists(GameDirectory + @"Data\XML"))
                    Directory.CreateDirectory(GameDirectory + @"Data\XML");

                if (File.Exists(GameDirectory + @"Data\XML\GAMECONSTANTS.XML"))
                    File.Delete(GameDirectory + @"Data\XML\GAMECONSTANTS.XML");
                if (File.Exists(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML"))
                    File.Delete(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML");

                File.WriteAllText(GameDirectory + @"Data\XML\GAMECONSTANTS.XML", Resources.GAMECONSTANTS);
                File.WriteAllText(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML", Resources.GRAPHICDETAILS);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool IsPatched()
        {
            if (!File.Exists(GameDirectory + @"Data\XML\GAMECONSTANTS.XML") ||
                !File.Exists(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML"))
                return false;
            var hashProvider = new HashProvider();
            if (hashProvider.GetFileHash(GameDirectory + @"Data\XML\GAMECONSTANTS.XML") !=
                Configuration.Config.GameconstantsUpdateHash)
                return false;
            if (hashProvider.GetFileHash(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML") !=
                Configuration.Config.GraphicdetailsUpdateHash)
                return false;
            return true;
        }

        public void DeleteMod(string name)
        {
            throw new NotImplementedException();
        }

        public void ClearDataFolder()
        {
            throw new NotImplementedException();
        }
    }
}
