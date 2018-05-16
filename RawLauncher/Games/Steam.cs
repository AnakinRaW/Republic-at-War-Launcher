using System;
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
            path = SteamExePath;
            return !string.IsNullOrEmpty(path);
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
    }
}
