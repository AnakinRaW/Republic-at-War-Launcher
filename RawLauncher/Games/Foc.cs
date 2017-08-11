using System;
using System.Diagnostics;
using System.IO;
using RawLauncher.Framework.Hash;
using RawLauncher.Framework.Helpers;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Games
{
    public class Foc : IGame
    {
        public const string GameconstantsUpdateHash = "4306d0c45d103cd11ff6743d1c3d9366";
        public const string GraphicdetailsUpdateHash = "4d7e140887fc1dd52f47790a6e20b5c5";

        public Foc() {}

        public Foc(string gameDirectory)
        {
            GameDirectory = gameDirectory;
            if (!Exists())
                throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExist"));
            GameProcessData = new GameProcessData();
        }

        public void ClearDataFolder()
        {
            if (Directory.Exists(@"Data\CustomMaps"))
                FileUtilities.DeleteDirectory(@"Data\CustomMaps");
            if (Directory.Exists(@"Data\Scripts"))
                FileUtilities.DeleteDirectory(@"Data\Scripts");
            if (Directory.Exists(@"Data\XML"))
                FileUtilities.DeleteDirectory(@"Data\XML");
        }

        public void BackUpAiFiles()
        {
            ClearBackupFiles();
            if (Directory.Exists(@"Data\CustomMaps"))
                Directory.Move(@"Data\CustomMaps", @"Data\CustomMapsBackup");
            if (Directory.Exists(@"Data\Scripts"))
                Directory.Move(@"Data\Scripts", @"Data\ScriptsBackup");
            if (Directory.Exists(@"Data\Xml"))
                Directory.Move(@"Data\Xml", @"Data\XmlBackup");
        }

        public void ResotreAiFiles()
        {
            ClearDataFolder();
            if (Directory.Exists(@"Data\CustomMapsBackup"))
                Directory.Move(@"Data\CustomMapsBackup", @"Data\CustomMaps");
            if (Directory.Exists(@"Data\ScriptsBackup"))
                Directory.Move(@"Data\ScriptsBackup", @"Data\Scripts");
            if (Directory.Exists(@"Data\XmlBackup"))
                Directory.Move(@"Data\XmlBackup", @"Data\Xml");
            ClearBackupFiles();
        }

        public void ClearBackupFiles()
        {
            if (Directory.Exists(@"Data\CustomMapsBackup"))
                Directory.Delete(@"Data\CustomMapsBackup", true);
            if (Directory.Exists(@"Data\ScriptsBackup"))
                Directory.Delete(@"Data\ScriptsBackup", true);
            if (Directory.Exists(@"Data\XmlBackup"))
                Directory.Delete(@"Data\XmlBackup", true);
        }

        public void DeleteMod(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (Directory.Exists(@"Mods\" + name))
                FileUtilities.DeleteDirectory(@"Mods\" + name);
        }

        public GameProcessData GameProcessData { get; }
        public bool Exists() => File.Exists(GameDirectory + @"\swfoc.exe");

        public IGame FindGame()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\swfoc.exe"))
                throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExistName", Name));
            return new Foc(Directory.GetCurrentDirectory() + @"\");
        }

        public string GameDirectory { get; }

        public bool IsPatched()
        {
            if (!File.Exists(GameDirectory + @"Data\XML\GAMECONSTANTS.XML") ||
                !File.Exists(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML"))
                return false;
            var hashProvider = new HashProvider();
            if (hashProvider.GetFileHash(GameDirectory + @"Data\XML\GAMECONSTANTS.XML") != GameconstantsUpdateHash)
                return false;
            if (hashProvider.GetFileHash(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML") != GraphicdetailsUpdateHash)
                return false;
            return true;
        }

        public string Name => "Forces of Corruption";

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

                File.WriteAllText(GameDirectory + @"Data\XML\GAMECONSTANTS.XML", Properties.Resources.GAMECONSTANTS);
                File.WriteAllText(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML", Properties.Resources.GRAPHICDETAILS);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
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
                GameProcessData.Process = process;
                GameProcessData.StartProcess();
            }
            catch (Exception)
            {
                //ignored
            }
        }

        public void PlayGame(IMod mod)
        {
            if (!mod.Exists())
                throw new ModExceptions(MessageProvider.GetMessage("ExceptionGameModExist"));
            if (!mod.ModDirectory.StartsWith(GameDirectory))
                throw new ModExceptions(MessageProvider.GetMessage("ExceptionGameModCompatible"));
            if (!mod.HasCorrectFolderStructure)
                throw new ModExceptions(MessageProvider.GetMessage("ExceptionGameModWrongInstalled"));

            FileShuffler.ShuffleFiles(mod.ModDirectory + @"\Data\");

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
                GameProcessData.Process = process;
                GameProcessData.StartProcess();
            }
            catch (Exception)
            {
                //ignored
            }
        }

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
    }
}