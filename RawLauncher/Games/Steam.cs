using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace RawLauncher.Framework.Games
{
    public static class Steam
    {
        public static string SteamExePath
        {
            get
            {
                try
                {
                    return Registry.CurrentUser.CreateSubKey("Software\\Valve\\Steam", RegistryKeyPermissionCheck.ReadSubTree)?.GetValue("SteamExe", null).ToString();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        public static bool IsSteamInstalled(out string path)
        {
            Log.Write("Check if steam is installed...");
            path = SteamExePath;
            Log.Write("Steam installed: " + (!string.IsNullOrEmpty(path)));
            return !string.IsNullOrEmpty(path);
        }

        public static bool IsSteamRunning()
        {
            Log.Write("Check if steam is running...");
            if (!IsSteamInstalled(out _))
                return false;

            using (var registry = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
            {
                Log.Write("Checking registry 'ActiveProcess' node");
                var steamKey = registry.OpenSubKey("Software\\Valve\\Steam\\ActiveProcess", false);
                if (steamKey == null)
                    return false;
                Log.Write("Checking registry 'pid' key");
                var pid = (int)steamKey.GetValue("pid");
                if (pid == 0)
                    return false;
                Log.Write("Checking pid is running");
                if (ProcessHelper.GetProcessByPid(pid) == null)
                    return false;
                return true;
            }
        }

        public static bool IsUserLoggedIn(out int userId)
        {
            Log.Write("Check for logged in user...");
            userId = -1;
            if (!IsSteamInstalled(out _))
                return false;

            using (var registry = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
            {
                var steamKey = registry.OpenSubKey("Software\\Valve\\Steam\\ActiveProcess", false);
                var t = steamKey?.GetValue("ActiveUser");
                if (t == null || !int.TryParse(t.ToString(), out userId))
                {
                    Log.Write("No user logged in");
                    return false;
                }

                Log.Write("Current user: " +  userId);
                return userId > 0;
            }
        }

        public static void WaitUserChanged(int ticks)
        {
            Log.Write("Wait for user logged in");
            IsUserLoggedIn(out var lastUserId);
            if (lastUserId > 0)
                return;

            var tick = 0;
            Log.Write("Looping:");
            while (tick++ < tick)
            {
                Log.Write("Current tick: " + tick);
                IsUserLoggedIn(out var currentUser);

                Log.Write("Was user changed: " + (currentUser != lastUserId));
                if (currentUser == 0 || currentUser == lastUserId)
                    continue;
                Log.Write("User logged in: " + currentUser);
                return;
            }
        }


        public static bool IsSteamGoldPackInstalled()
        {
            if (!IsSteamInstalled(out _))
                return false;

            using (var registry = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default))
            {
                return registry.OpenSubKey("Software\\Valve\\Steam\\Apps\\32470", false) != null ||
                       registry.OpenSubKey("Software\\Valve\\Steam\\Apps\\32472", false) != null;
            }
        }

        internal static void StartSteam()
        {
            Log.Write("Starting Steam");
            if (!IsSteamInstalled(out var steamPath))
                return;
            var process = new Process
            {
                StartInfo =
                {
                    FileName = steamPath,
                    UseShellExecute = false
                }
            };
            Log.Write("Starting Steam process...");
            process.Start();
            Log.Write("Steam process started");
            WaitUserChanged(3000);
        }
    }
}
