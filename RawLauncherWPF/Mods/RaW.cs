using System;
using System.IO;
using System.Windows;
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
        public bool HasCorrectFolderStructure => ModDirectory.Contains(@"Mods\" + _modFolderName);

        public string LaunchArgumentPath => "Mods/" + _modFolderName;

        public bool Exists()
        {
            return File.Exists(ModDirectory + @"\XML\Gameobjectfiles.xml");
        }

        public IMod FindMod()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\Mods\Republic_at_War\Data\XML\Gameobjectfiles.xml"))
                throw new ModExceptions(Name + " does not exists");
            return new RaW(Directory.GetCurrentDirectory() + @"\Mods\Republic_at_War\Data\");
        }

        public string ModDirectory { get; }
        public string Name => "Republic at War";

        public string Version
        {
            get
            {
                try
                {
                    var node = XmlTools.GetNodeValue(ModDirectory + @"\XML\Gameobjectfiles.xml",
                        "/Game_Object_Files/Version");
                    return string.IsNullOrEmpty(node) ? "1.1.5" : node;
                }
                catch (Exception)
                {
                    MessageBox.Show(
                        "Could not get the current version. Please reinstall the Republic at War and try again.");
                    return null;
                }
            }
            set { throw new NotImplementedException(); }
        }

        private string _modFolderName => "Republic_at_War";
    }
}