using RawLauncherWPF.Hash;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using static RawLauncherWPF.Games.Steam;
using static RawLauncherWPF.Utilities.MessageProvider;

namespace RawLauncherWPF.Games
{
    public sealed class SteamGame : IGame
    {
        public const string GameconstantsUpdateHash = "4306d0c45d103cd11ff6743d1c3d9366";
        public const string GraphicdetailsUpdateHash = "4d7e140887fc1dd52f47790a6e20b5c5";

        public SteamGame()
        {
        }

        public SteamGame(string gameDirectory)
        {
            GameDirectory = gameDirectory;
            if (!Exists())
                throw new GameExceptions(GetMessage("ExceptionGameExist"));
            GameProcessData = new GameProcessData();
        }

        public void ClearDataFolder()
        {
            if (Directory.Exists(@"Data\CustomMaps"))
                Directory.Delete(@"Data\CustomMaps", true);
            if (Directory.Exists(@"Data\Scripts"))
                Directory.Delete(@"Data\Scripts", true);
            if (Directory.Exists(@"Data\XML"))
                Directory.Delete(@"Data\XML", true);
        }

        public void DeleteMod(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (Directory.Exists(@"Mods\" + name))
                Directory.Delete(@"Mods\" + name, true);
        }

        public GameProcessData GameProcessData { get; }

        public bool Exists() => File.Exists(GameDirectory + @"\swfoc.exe");

        public IGame FindGame()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\swfoc.exe"))
                throw new GameExceptions(GetMessage("ExceptionGameExistName", Name));
            return new SteamGame(Directory.GetCurrentDirectory() + @"\");
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

        public string Name => "Forces of Corruption (Steam)";

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

        public void PlayGame()
        {
            PlayGame(null);
        }

        public void PlayGame(IMod mod)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = SteamExePath,
            };

            if (mod == null)
                startInfo.Arguments = "-applaunch 32470 swfoc";
            else
                startInfo.Arguments = "-applaunch 32470 swfoc MODPATH=" + mod.LaunchArgumentPath;

            string str = Directory.GetParent(new DirectoryInfo(Directory.GetCurrentDirectory()).FullName).FullName;

            if (!Exists())
                throw new GameExceptions(GetMessage("ExceptionGameExistName", Name));

            File.Move(str + "\\runme.dat", str + "\\tmp.runme.dat.tmp");
            File.Copy(str + "\\runm2.dat", str + "\\runme.dat");
            Process.Start(startInfo);
            Thread.Sleep(2000);
            File.Delete(str + "\\runme.dat");
            File.Move(str + "\\tmp.runme.dat.tmp", str + "\\runme.dat");

            GameProcessData.Process = ProcessHelper.FindProcess("swfoc");
        }

        public string SaveGameDirectory
        {
            get
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    @"Saved Games\Petroglyph\Empire At War - Forces of Corruption\Save\");
                if (!Directory.Exists(folder))
                    return "";
                return folder;
            }
        }
    }
}