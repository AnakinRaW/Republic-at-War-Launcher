using System;
using System.IO;
using RawLauncherWPF.Utilities;

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
                throw new ModExceptions("This Mod does not exists");
        }

        /// <summary>
        /// Checks if the Mod is in Fodlder "../Mods/ModName"
        /// </summary>
        public bool HasCorrectFolderStructure => ModDirectory.Contains(@"Mods\" + FolderName);

        public string LaunchArgumentPath => "Mods/" + FolderName;

        public string FolderName => "Republic_at_War";

        public bool Exists()
        {
            return File.Exists(ModDirectory + @"Data\XML\Gameobjectfiles.xml");
        }

        public IMod FindMod()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\Mods\Republic_at_War\Data\XML\Gameobjectfiles.xml"))
                throw new ModExceptions(Name + " does not exists");
            return new RaW(Directory.GetCurrentDirectory() + @"\Mods\Republic_at_War\");
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
                    MessageProvider.Show(
                        "Could not get the current version. Please reinstall the Republic at War and try again.");
                    return null;
                }
            }
            set {  }
        }
    }
}