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
            return CheckSteam(path) ? GameTypes.SteamGold : GameTypes.Disk;
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
    }
}
