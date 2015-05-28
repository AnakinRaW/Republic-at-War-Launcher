using System.IO;

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
    }
}
