using System.Collections.Generic;
using RawLauncherWPF.Localization;

namespace RawLauncherWPF.Configuration
{
    /// <summary>
    /// This Class provides constant datas the Launcher will read. 
    /// </summary>
    internal static class Config
    {
        internal static Language CurrentLanguage { get; set; }

        public const string EeawForum = "http://www.everythingeaw.com/forum/index.php";
        public const string ModdbPage = "http://www.moddb.com/mods/republic-at-war";
        public const string FastLaunchFileName = "RawLauncherInfo.txt";
        public const string ServerUrl = "https://raw.githubusercontent.com/Republic-at-War/Republic-At-War/master/";
        public const string VersionListRelativePath = @"\Versions\AvailableVersions.txt";
        public const string SessionServerUrl = "http://raworganize.com/";

        #region Beta Stuff
        internal static Dictionary<string, string> BetaUsers = new Dictionary<string, string>
        {
            {"Anakin", "d783bdc09ad8a57e4afaf4243fd0af7c"}
        };

        internal const string BetaPassword = "26fb2b525ebc73751cb420e0180b7acc";

        #endregion


    }
}
