using System.IO;
using Microsoft.Win32;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Games
{
    public static class GameHelper
    {

        public struct GameHelperResult
        {
            public GameTypes Type { get; }

            public string FocPath { get; }

            public GameHelperResult(GameTypes type, string path)
            {
                Type = type;
                FocPath = path;
            }
        }


        public static GameHelperResult GetInstalledGameType(string path)
        {
            string focPath;

            if (File.Exists(path + "\\swfoc.exe"))
                focPath = path;
            else
            {
                var installed = (int)Registry.LocalMachine
                    .OpenSubKey(@"SOFTWARE\LucasArts\Star Wars Empire at War Forces of Corruption\1.0", false).GetValue("installed");
                if (installed == 0)
                    throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExist"));
                var exePath = (string)Registry.LocalMachine
                    .OpenSubKey(@"SOFTWARE\LucasArts\Star Wars Empire at War Forces of Corruption\1.0", false)?.GetValue("exepath");
                focPath = new FileInfo(exePath).Directory.FullName;
            }
            if (focPath == null || !File.Exists(focPath + "\\swfoc.exe"))
                throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExist"));

            if (CheckSteam(focPath))
                return new GameHelperResult(GameTypes.SteamGold, focPath);
            return CheckGoG(focPath)
                ? new GameHelperResult(GameTypes.GoG, focPath)
                : new GameHelperResult(GameTypes.Disk, focPath);
        }

        private static bool CheckSteam(string path)
        {
            if (new DirectoryInfo(path).Name != "corruption")
                return false;
            if (!Directory.Exists(Directory.GetParent(path).FullName + "\\GameData\\"))
                return false;
            if (!File.Exists(Directory.GetParent(path) + "\\GameData\\sweaw.exe"))
                return false;
            if (!File.Exists(Directory.GetParent(path) + "\\runm2.dat") ||
                !File.Exists(Directory.GetParent(path) + "\\runme.dat"))
                return false;
            return true;
        }

        private static bool CheckGoG(string path)
        {
            if (new DirectoryInfo(path).Name != "EAWX")
                return false;
            if (!File.Exists(Directory.GetParent(path) + "\\GameData\\sweaw.exe"))
                return false;
            if (!File.Exists(Directory.GetParent(path) + "\\GameData\\goggame-1421404887.dll"))
                return false;
            return true;
        }
    }
}
