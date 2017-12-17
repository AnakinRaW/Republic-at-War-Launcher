using System.IO;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Games
{
    public static class GameHelper
    {
        public static GameTypes GetInstalledGameType(string path)
        {
            if (!File.Exists(path + "\\swfoc.exe"))
                throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExist"));

            if (CheckSteam(path))
                return GameTypes.SteamGold;
            return CheckGoG(path) ? GameTypes.GoG : GameTypes.Disk;
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
