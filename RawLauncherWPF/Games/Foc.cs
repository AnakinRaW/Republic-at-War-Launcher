using System;
using System.Diagnostics;
using System.IO;
using RawLauncherWPF.Hash;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Properties;
using static RawLauncherWPF.Utilities.MessageProvider;

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
                throw new GameExceptions(GetMessage("ExceptionGameExist"));
        }

        public string GameDirectory { get; }

        public string SaveGameDirectory
        {
            get
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Petroglyph\Empire At War - Forces of Corruption\Save\");
                if (!Directory.Exists(folder))
                    return "";
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Petroglyph\Empire At War - Forces of Corruption\Save\");
            }
        }

        public string Name => "Forces of Corruption";

        public bool Exists() => File.Exists(GameDirectory + @"\swfoc.exe");

        public IGame FindGame()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\swfoc.exe"))
                throw new GameExceptions(GetMessage("ExceptionGameExistName", Name));
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
                throw new ModExceptions(GetMessage("ExceptionGameModExsit"));
            if (!mod.ModDirectory.StartsWith(GameDirectory))
                throw new ModExceptions(GetMessage("ExceptionGameModCompatible"));
            if (!mod.HasCorrectFolderStructure)
                throw new ModExceptions(GetMessage("ExceptionGameModWrongInstalled"));

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
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (Directory.Exists(@"Mods\" + name))
                Directory.Delete(@"Mods\" + name, true);
        }

        public void ClearDataFolder()
        {
            if (Directory.Exists(@"Data\CustomMaps"))
                Directory.Delete(@"Data\CustomMaps");
            if (Directory.Exists(@"Data\Scripts"))
                Directory.Delete(@"Data\Scripts");
            if (Directory.Exists(@"Data\XML"))
                Directory.Delete(@"Data\XML");
        }
    }
}
