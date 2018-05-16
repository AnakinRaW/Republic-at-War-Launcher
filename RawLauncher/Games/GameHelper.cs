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

            FindGamesFromRegistry(ref result);
            if (result.IsError)
                return result;

            //CheckViaPath -- override registry if found.
            FindGamesFromPath(path, ref result);

            if (string.IsNullOrEmpty(result.FocPath) || !File.Exists(Path.Combine(result.FocPath + "\\swfoc.exe")))
            {
                result.IsError = true;
                result.Error = DetectionError.NotInstalled;
                return result;
            }

            if (CheckSteam(result.FocPath))
                result.FocType = GameTypes.SteamGold;
            else if (CheckGoG(result.FocPath))
                result.FocType = GameTypes.GoG;
            else
                result.FocType = GameTypes.Disk;
            return result;
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

        private static void FindGamesFromPath(string path, ref GameDetectionResult result)
        {
            //throw new System.NotImplementedException();
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
