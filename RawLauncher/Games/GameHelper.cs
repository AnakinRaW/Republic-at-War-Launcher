using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using RawLauncher.Framework.Utilities;
using static RawLauncher.Framework.Configuration.Config;

namespace RawLauncher.Framework.Games
{
    public static class GameHelper
    {
        internal static GameDetectionResult GetInstalledGameType(string path)
        {
            var result = default(GameDetectionResult);

            string focPath;

            if (File.Exists(path + "\\swfoc.exe"))
                focPath = path;
            else
            {
                using (var registry = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                {
                    var key = registry.OpenSubKey(FocRegistryPath + FocRegistryVersion, false);
                    if (key == null)
                    {
                        var baseKey = registry.OpenSubKey(FocRegistryPath, false);
                        if (baseKey == null)
                        {
                            result.IsError = true;
                            result.Error = DetectionError.NotInstalled;
                            return result;
                        }

                        if (!PrompGameSetupDialog(ref result))
                            return result;
                        SetupSteamGames(in registry, ref key);  
                    }

                    if (key == null)
                    {
                        result.IsError = true;
                        result.Error = DetectionError.NotSettedUp;
                        return result;
                    }

                    var installed = (int)key.GetValue("installed");
                    if (installed == 0)
                    {
                        result.IsError = true;
                        result.Error = DetectionError.NotInstalled;
                        return result;
                    }
                    var exePath = (string)registry.OpenSubKey(FocRegistryPath + FocRegistryVersion, false)?.GetValue("exepath");
                    focPath = new FileInfo(exePath).Directory.FullName;
                }
            }
            if (focPath == null || !File.Exists(focPath + "\\swfoc.exe"))
            {
                result.IsError = true;
                result.Error = DetectionError.NotInstalled;
                return result;
            }

            result.FocPath = focPath;
            if (CheckSteam(focPath))
            {
                result.Type = GameTypes.SteamGold;
                return result;
            }
            if (CheckGoG(focPath))
            {
                result.Type = GameTypes.GoG;
                return result;
            }
            result.Type = GameTypes.Disk;
            return result;
        }

        private static void SetupSteamGames(in RegistryKey registry, ref RegistryKey key)
        {
            ProcessHelper.FindProcess("StarWarsG")?.Kill();
            Process.Start("steam://rungameid/32470");

            for (var count = 0; count <= 5000; count++)
            {
                var eaw = ProcessHelper.FindProcess("StarWarsG");
                if (eaw != null)
                {
                    key = registry.OpenSubKey(FocRegistryPath + FocRegistryVersion, false);
                    if (key != null)
                    {
                        eaw.Kill();
                        break;
                    }
                }
            }
        }

        private static bool PrompGameSetupDialog(ref GameDetectionResult result)
        {
            if (!Steam.IsSteamInstalled(out _))
            {
                result.IsError = true;
                result.Error = DetectionError.NotSettedUp;
                return false;
            }
            var mbResult = MessageBox.Show(MessageProvider.GetMessage("WarningGamesSettedUp"), "Republic at War", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes);
            if (mbResult == MessageBoxResult.Yes)
                return true;
            result.IsError = true;
            result.Error = DetectionError.NotSettedUp;
            return false;
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
