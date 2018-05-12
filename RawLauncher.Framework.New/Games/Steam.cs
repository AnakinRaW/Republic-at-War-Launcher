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
    }
}
