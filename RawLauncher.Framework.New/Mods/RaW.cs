using System;
using System.Globalization;
using System.IO;
using System.Linq;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.Xml;

namespace RawLauncher.Framework.Mods
{
    public class RaW : IMod
    {
        public RaW()
        {
        }

        public RaW(string modDirectory, bool workshopMod = false)
        {
            ModDirectory = modDirectory;
            if (!Exists())
                throw new ModExceptions(MessageProvider.GetMessage("ExceptionModExist"));
            WorkshopMod = workshopMod;
            FolderName = new DirectoryInfo(ModDirectory).Name;
        }

        public string FolderName { get; }


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

        public bool WorkshopMod { get; }

        public bool Exists()
        {
            return File.Exists(ModDirectory + @"Data\XML\Gameobjectfiles.xml");
        }

        public IMod FindMod(IGame baseGame)
        {
            if (File.Exists(baseGame.GameDirectory + @"\Mods\Republic_At_War\Data\XML\Gameobjectfiles.xml"))
                return new RaW(baseGame.GameDirectory + @"\Mods\Republic_At_War\");

            var dir = Path.Combine(baseGame.GameDirectory, @"..\..\..\workshop\content\32470\1129810972");
            var d = new DirectoryInfo(dir).FullName + "\\";

            //MessageBox.Show(d);
            if (!Directory.Exists(d))
                throw new ModExceptions(MessageProvider.GetMessage("ExceptionModExistName", Name));
            return new RaW(d, true);
            //var modfile =
            //    new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\workshop\content\32470\"))
            //        .GetFiles("Republic at War Changelog.txt", SearchOption.AllDirectories).FirstOrDefault();
            //if (modfile == null)
            //    throw new ModExceptions(MessageProvider.GetMessage("ExceptionModExistName", Name));
            //return new RaW(modfile.Directory.FullName + "\\", true);
        }

        public void PrepareStart(IGame game)
        {
            //if (!Directory.Exists(Path.Combine(ModDirectory, "RootDataFiles")))
            //{
            //    CleanUpAfterGameClose = false;
            //    return;
            //}
            //game.BackUpAiFiles();
            //if (Directory.Exists(Path.Combine(ModDirectory, @"RootDataFiles\CustomMaps")))
            //    FileUtilities.Copy(Path.Combine(ModDirectory, @"RootDataFiles\CustomMaps"), Path.Combine(game.GameDirectory, @"Data\CustomMaps\"));
            //if (Directory.Exists(Path.Combine(ModDirectory, @"RootDataFiles\Scripts")))
            //    FileUtilities.Copy(Path.Combine(ModDirectory, @"RootDataFiles\Scripts"), Path.Combine(game.GameDirectory, @"Data\Scripts\"));
            //if (Directory.Exists(Path.Combine(ModDirectory, @"RootDataFiles\XML")))
            //    FileUtilities.Copy(Path.Combine(ModDirectory, @"RootDataFiles\XML"), Path.Combine(game.GameDirectory, @"Data\XML\"));
            //CleanUpAfterGameClose = true;
        }

        public void CleanUpAferGame(IGame game)
        {
            //if (!CleanUpAfterGameClose)
            //    return;
            //game.ClearDataFolder();
            //game.ResotreAiFiles();
            //File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "tmp.lnk"));
        }

        public void Delete()
        {
            FileUtilities.DeleteDirectory(ModDirectory);
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
            set { }
        }
    }
}