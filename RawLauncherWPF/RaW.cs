using System;
using System.IO;
using System.Windows;

namespace RawLauncherWPF
{
    public class RaW : Mod
    {
        public RaW()
        {
        }

        public RaW(string modDirectory) : base(modDirectory)
        {
        }

        public override string Name => "Republic at War";

        public override Mod FindMod()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\Mods\Republic_at_War\Data\XML\Gameobjectfiles.xml"))
                throw new ModExceptions(Name + " does not exists");
            return new RaW(Directory.GetCurrentDirectory() + @"\Mods\Republic_at_War\Data\");
        }

        public override string Version
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
                    MessageBox.Show("Could not get the current version. Please reinstall the Republic at War and try again.");
                    return null;
                }
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
