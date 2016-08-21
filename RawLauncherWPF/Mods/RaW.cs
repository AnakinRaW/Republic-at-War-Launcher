using System;
using System.Globalization;
using System.IO;
using System.Linq;
using RawLauncherWPF.Utilities;
using RawLauncherWPF.ViewModels;
using static RawLauncherWPF.Utilities.MessageProvider;

namespace RawLauncherWPF.Mods
{
    public class RaW : IMod
    {
        public RaW()
        {
        }

        public RaW(string modDirectory)
        {
            ModDirectory = modDirectory;
            if (!Exists())
                throw new ModExceptions(GetMessage("ExceptionModExist"));
        }

        /// <summary>
        /// Checks if the Mod is in Fodlder "../Mods/ModName"
        /// </summary>
        public bool HasCorrectFolderStructure => ModDirectory.Contains(@"Mods\" + FolderName);

        public string LaunchArgumentPath => "Mods/" + FolderName;

        public string FolderName => "Republic_At_War";

        public LanguageTypes InstalledLanguage
        {
            get
            {
                if (Directory.EnumerateFiles(ModDirectory + @"Data\Text", "MasterTextFile_*.dat", SearchOption.AllDirectories).Count() < 0)
                    return LanguageTypes.None;
                var s =
                    Path.GetFileName(
                        Directory.EnumerateFiles(ModDirectory + @"Data\Text", "MasterTextFile*.dat",
                            SearchOption.AllDirectories).First());
                var n = s?.Replace("MasterTextFile_", "").Replace(".dat", "").Replace(".DAT", "");
                n = n?.ToLower();
                n = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(n);

                LanguageTypes result;
                try
                {
                    result = (LanguageTypes)Enum.Parse(typeof(LanguageTypes), n);
                }
                catch (Exception)
                {
                    result = LanguageTypes.None;
                }
                return result;
            }
        }

        public bool Exists()
        {
            return File.Exists(ModDirectory + @"Data\XML\Gameobjectfiles.xml");
        }

        public IMod FindMod()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\Mods\Republic_At_War\Data\XML\Gameobjectfiles.xml"))
                throw new ModExceptions(GetMessage("ExceptionModExistName", Name));
            return new RaW(Directory.GetCurrentDirectory() + @"\Mods\Republic_At_War\");
        }

        public void PrepareStart()
        {
            Show("Prepare");
        }

        public void CleanUpAferGame()
        {
            Show("Clean");
        }

        public string ModDirectory { get; }
        public string Name => "Republic at War";

        public Version Version
        {
            get
            {
                try
                {
                    var node = XmlTools.GetNodeValue(ModDirectory + @"\Data\XML\Gameobjectfiles.xml",
                        "/Game_Object_Files/Version");
                    if (string.IsNullOrEmpty(node))
                        return new Version("1.1.5");
                    return new Version(node);
                }
                catch (Exception)
                {
                    Show(GetMessage(
                        "ModVersionNotFound"));
                    return null;
                }
            }
            set {  }
        }
    }
}