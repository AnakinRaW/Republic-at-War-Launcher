using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using ModernApplicationFramework.Utilities;
using RawLauncher.Framework.Utilities;
using static RawLauncher.Framework.Configuration.Config;

namespace RawLauncher.Framework.Games
{
    public static class GameHelper
    {
        internal static GameDetectionResult GetGameInstallations()
        {
            var result = default(GameDetectionResult);

            FindGamesFromRegistry(ref result);
            if (result.IsError)
                return result;

            FindGamesFromExecutingPath(ref result);

            if (string.IsNullOrEmpty(result.FocPath) || !File.Exists(Path.Combine(result.FocPath + "\\swfoc.exe")))
            {
                result.IsError = true;
                result.Error = DetectionError.NotInstalled;
                return result;
            }
            result.FocType = GetGameType(in result);
            return result;
        }

        private static GameType GetGameType(in GameDetectionResult result)
        {
            if (CheckSteam(result.FocPath))
                return GameType.SteamGold;
            if (CheckGoG(result.FocPath))
                return GameType.GoG;
            return GameType.Disk;
        }

        private static void FindGamesFromRegistry(ref GameDetectionResult result)
        {
            var eawResult = CheckGameExists(EawRegistryPath, EawRegistryVersion);
            var focResult = CheckGameExists(FocRegistryPath, FocRegistryVersion);

            if (eawResult == DetectionError.None && focResult == DetectionError.None)
            {
                result.EawPath = GetGamePathFromRegistry(EawRegistryPath + EawRegistryVersion);
                result.FocPath = GetGamePathFromRegistry(FocRegistryPath + FocRegistryVersion);
                return;
            }
            if (eawResult == DetectionError.NotInstalled || focResult == DetectionError.NotInstalled)
            {
                result.IsError = true;
                result.Error = DetectionError.NotInstalled;
                return;
            }
            if (eawResult == DetectionError.NotSettedUp || focResult == DetectionError.NotSettedUp)
            {
                if (Steam.IsSteamGoldPackInstalled() && PrompGameSetupDialog() && SetupSteamGames())
                {
                    result.EawPath = GetGamePathFromRegistry(EawRegistryPath + EawRegistryVersion);
                    result.FocPath = GetGamePathFromRegistry(FocRegistryPath + FocRegistryVersion);
                    return;
                }
                result.IsError = true;
                result.Error = DetectionError.NotSettedUp;
            }
        }

        private static void FindGamesFromExecutingPath(ref GameDetectionResult result)
        {
            var currentPath = Directory.GetCurrentDirectory();

            if (!File.Exists(Path.Combine(currentPath, "swfoc.exe")))
                return;

            if (result.FocPath.NormalizePath() == currentPath.NormalizePath())
                return;

            var newResult = default(GameDetectionResult);
            newResult.FocPath = currentPath;

            var gameType = GetGameType(in newResult);
            newResult.FocType = gameType;
            if (!Eaw.FindInstallationRelativeToFoc(newResult.FocPath, gameType, out var eawPath))
                return;
            newResult.EawPath = eawPath;
            
            result = newResult;
        }

        private static DetectionError CheckGameExists(string baseKeyPath, string versionKeyPath)
        {
            using (var registry = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                var key = registry.OpenSubKey(baseKeyPath + versionKeyPath, false);
                if (key == null)
                {
                    var baseKey = registry.OpenSubKey(baseKeyPath, false);
                    return baseKey == null ? DetectionError.NotInstalled : DetectionError.NotSettedUp;
                }
                var installed = (int)key.GetValue("installed");
                return installed == 0 ? DetectionError.NotInstalled : DetectionError.None;
            }
        }

        private static string GetGamePathFromRegistry(string registryPath)
        {
            using (var registry = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            {
                var exePath = (string)registry.OpenSubKey(registryPath, false)?.GetValue("exepath");
                return new FileInfo(exePath).Directory.FullName;
            }
        }

        private static bool SetupSteamGames()
        {
            ProcessHelper.FindProcess("StarWarsG")?.Kill();
            Process.Start("steam://rungameid/32470");

            for (var count = 0; count <= 5000; count++)
            {
                var eaw = ProcessHelper.FindProcess("StarWarsG");
                if (eaw != null)
                {
                    if (CheckGameExists(EawRegistryPath, EawRegistryVersion) == DetectionError.None &&
                        CheckGameExists(FocRegistryPath, FocRegistryVersion) == DetectionError.None)
                    {
                        eaw.Kill();
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool PrompGameSetupDialog()
        {
            var mbResult = MessageBox.Show(MessageProvider.GetMessage("WarningGamesSettedUp"), "Republic at War", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes);
            return mbResult == MessageBoxResult.Yes;
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
