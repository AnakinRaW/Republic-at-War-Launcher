using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using RawLauncherWPF.Hash;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Properties;
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

        public void DeleteMod(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (Directory.Exists(@"Mods\" + name))
                Directory.Delete(@"Mods\" + name, true);
        }

        public bool Exists() => File.Exists(GameDirectory + @"\swfoc.exe");

        public IGame FindGame()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\swfoc.exe"))
                throw new GameExceptions(GetMessage("ExceptionGameExistName", Name));
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
            if (SteamExePath == string.Empty)
                throw new GameExceptions(GetMessage("ExceptionSteamClientMissing"));

            var process = new ProcessStartInfo {FileName = SteamExePath};
            if (mod == null)
                process.Arguments = "-applaunch 32470 swfoc";
            else
                process.Arguments = "-applaunch 32470 swfoc " + mod.LaunchArgumentPath;

            var steamRoot = Directory.GetParent(GameDirectory);
            if (!File.Exists(steamRoot + "\\runme.dat") || !File.Exists(steamRoot + "\\runm2.dat"))
                throw new GameExceptions(GetMessage("ExceptionGameExistName", Name));
            File.Move(steamRoot + "\\runme.dat", steamRoot + "\\tmp.runme.dat.tmp");
            File.Copy(steamRoot + "\\runm2.dat", steamRoot + "\\runme.dat");
            Process.Start(process);
            Thread.Sleep(20000);
            File.Delete(steamRoot + "\\runme.dat");
            File.Move(steamRoot + "\\tmp.runme.dat.tmp", steamRoot + "\\runme.dat");
        }

        //TODO
        public string SaveGameDirectory { get; }
    }
}