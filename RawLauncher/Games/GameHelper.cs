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


            Log.Write("Invoking FindGamesFromRegistry()");

            FindGamesFromRegistry(ref result);

            Log.Write("FindGamesFromRegistry() invoked with result: " + result);

            if (result.IsError)
                return result;

            result.FocType = GetGameType(ref result);

            Log.Write("Invoking FindGamesFromExecutingPath()");
            FindGamesFromExecutingPath(ref result);

            Log.Write("FindGamesFromExecutingPath() invoked with result: " + result);

            Log.Write("Checking if FoC Path is not null or empty && assuring 'swfoc.exe' exists.");
            if (string.IsNullOrEmpty(result.FocPath) || !File.Exists(Path.Combine(result.FocPath + "\\swfoc.exe")))
            {
                Log.Write("Check failed");
                result.IsError = true;
                result.Error = DetectionError.NotInstalled;
                return result;
            }
            return result;
        }

        private static GameType GetGameType(ref GameDetectionResult result)
        {
            Log.Write("Checking game type...");
            if (CheckSteam(result.FocPath))
            {
                Log.Write("Game type is Steam");
                return GameType.SteamGold;
            }

            if (CheckGoG(result.FocPath))
            {
                Log.Write("Game type is GoG");
                return GameType.GoG;
            }

            if (CheckOrigin(ref result))
            {
                Log.Write("Game type is Origin");
                return GameType.Origin;
            }

            Log.Write("Game type is Disk");
            return GameType.Disk;
        }

        private static void FindGamesFromRegistry(ref GameDetectionResult result)
        {
            var eawResult = CheckGameExists(EawRegistryPath, EawRegistryVersion);
            Log.Write("EaW registry check result: " + eawResult);

            var focResult = CheckGameExists(FocRegistryPath, FocRegistryVersion);
            Log.Write("FoC registry check result: " + focResult);

            if (eawResult == DetectionError.None && focResult == DetectionError.None)
            {
                result.EawPath = GetGamePathFromRegistry(EawRegistryPath + EawRegistryVersion);
                Log.Write("EaW path: " + result.EawPath);

                result.FocPath = GetGamePathFromRegistry(FocRegistryPath + FocRegistryVersion);
                Log.Write("FoC path: " + result.FocPath);
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
                Log.Write("One game was not set up - Checking for steam version and prompt start dialog");
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

            Log.Write("Checking if Path contains 'swfoc.exe'...");
            if (!File.Exists(Path.Combine(currentPath, "swfoc.exe")))
            {
                Log.Write("'swfoc.exe' was not found");
                return;
            }

            Log.Write("Checking if result path and current path are equal...");
            if (result.FocPath.NormalizePath() == currentPath.NormalizePath())
            {
                Log.Write("result path and current path are equal");
                return;
            }

            var newResult = default(GameDetectionResult);
            newResult.FocPath = currentPath;

            var gameType = GetGameType(ref newResult);
            newResult.FocType = gameType;

            if (!Eaw.FindInstallationRelativeToFoc(newResult.FocPath, gameType, out var eawPath))
            {
                newResult.EawPath = result.EawPath;
                result = newResult;
                return;
            }

            newResult.EawPath = eawPath;
            result = newResult;
        }

        private static DetectionError CheckGameExists(string baseKeyPath, string versionKeyPath)
        {
            Log.Write($"Checking if game exists in registry: {baseKeyPath}, {versionKeyPath}");
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
            Log.Write("Getting game path from registry: " + registryPath);
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
            Log.Write("Checking if game type is steam");
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
            Log.Write("Checking if game type is GoG");
            if (new DirectoryInfo(path).Name != "EAWX")
                return false;
            if (!File.Exists(Directory.GetParent(path) + "\\GameData\\sweaw.exe"))
                return false;
            if (!File.Exists(Directory.GetParent(path) + "\\GameData\\goggame-1421404887.dll"))
                return false;
            return true;
        }

        private static bool CheckOrigin(ref GameDetectionResult result)
        {
            Log.Write("Checking if game type is Origin");
            FixPossibleOriginBug(ref result);

            if (new DirectoryInfo(result.FocPath).Name != "EAWX")
                return false;
            if (!Directory.Exists(Path.Combine(Directory.GetParent(result.FocPath).FullName, "Manuals")))
                return false;
            if (!Directory.Exists(Path.Combine(Directory.GetParent(result.FocPath).FullName, "__Installer")))
                return false;
            return true;
        }


        private static void FixPossibleOriginBug(ref GameDetectionResult result)
        {
            var exeDir = new DirectoryInfo(result.FocPath);
            if (exeDir.Name == "corruption")
            {
                var parentPath = exeDir.Parent?.FullName;
                if (parentPath == null)
                    return;

                var correctedPath = Path.Combine(parentPath, "EAWX");
                if (Directory.Exists(correctedPath))
                    result.FocPath = correctedPath;
            }
        }
    }
}
