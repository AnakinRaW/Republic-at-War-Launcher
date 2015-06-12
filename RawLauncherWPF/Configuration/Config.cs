using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawLauncherWPF.Configuration
{
    /// <summary>
    /// This Class provides constant datas the Launcher will read. 
    /// TODO: Maybe make this Class to a local config file. So we can update it as well easily
    /// </summary>
    internal static class Config
    {
        public const string EeawForum = "http://www.everythingeaw.com/forum/index.php";
        public const string GameconstantsUpdateHash = "4306d0c45d103cd11ff6743d1c3d9366";
        public const string GameconstantsUpdateHashEaW = "1d44b0797c8becbe240adc0098c2302a";
        public const string GraphicdetailsUpdateHash = "4d7e140887fc1dd52f47790a6e20b5c5";
        public const string ModdbPage = "http://www.moddb.com/mods/republic-at-war";
        public const string Server = "";
        //TODO: Change to GitHub in future
        public const string ServerUrl = "http://raworganize.com/RaW_Update/";
        public static string CurrentDirectory => Directory.GetCurrentDirectory();
    }
}
