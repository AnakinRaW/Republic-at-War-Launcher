using System;
using System.Globalization;
using System.IO;
using System.Linq;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.ViewModels;
using RawLauncher.Framework.Xml;

namespace RawLauncher.Framework.Mods
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
                throw new ModExceptions(MessageProvider.GetMessage("ExceptionModExist"));
        }

        /// <summary>
        /// Checks if the Mod is in Fodlder "../Mods/ModName"
        /// </summary>
        public bool HasCorrectFolderStructure => ModDirectory.Contains(@"Mods\" + FolderName);

        public string LaunchArgumentPath => "Mods/" + FolderName;

        public string FolderName => "Republic_At_War";


        protected bool CleanUpAfterGameClose { get; set; }

        public LanguageTypes InstalledLanguage
        {
            get
            {
                if (!Directory.Exists(Path.Combine(ModDirectory, @"Data\Text")))
                    return LanguageTypes.None;
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
                throw new ModExceptions(MessageProvider.GetMessage("ExceptionModExistName", Name));
            return new RaW(Directory.GetCurrentDirectory() + @"\Mods\Republic_At_War\");
        }

        public void PrepareStart(IGame game)
        {
            if (!Directory.Exists(Path.Combine(ModDirectory, "RootDataFiles")))
            {
                CleanUpAfterGameClose = false;
                return;
            }
            game.BackUpAiFiles();
            if (Directory.Exists(Path.Combine(ModDirectory, @"RootDataFiles\CustomMaps")))
                FileUtilities.Copy(Path.Combine(ModDirectory, @"RootDataFiles\CustomMaps"), Path.Combine(game.GameDirectory, @"Data\CustomMaps\"));
            if (Directory.Exists(Path.Combine(ModDirectory, @"RootDataFiles\Scripts")))
                FileUtilities.Copy(Path.Combine(ModDirectory, @"RootDataFiles\Scripts"), Path.Combine(game.GameDirectory, @"Data\Scripts\"));
            if (Directory.Exists(Path.Combine(ModDirectory, @"RootDataFiles\XML")))
                FileUtilities.Copy(Path.Combine(ModDirectory, @"RootDataFiles\XML"), Path.Combine(game.GameDirectory, @"Data\XML\"));
            CleanUpAfterGameClose = true;
        }

        public void CleanUpAferGame(IGame game)
        {
            if (!CleanUpAfterGameClose)
                return;
            game.ClearDataFolder();
            game.ResotreAiFiles();
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
                    return string.IsNullOrEmpty(node) ? new Version("1.1.5") : new Version(node);
                }
                catch (Exception)
                {
                    MessageProvider.Show(MessageProvider.GetMessage(
                        "ModVersionNotFound"));
                    return null;
                }
            }
            set {  }
        }
    }
}