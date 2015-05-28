using System.IO;

namespace RawLauncherWPF
{
    public abstract class Mod
    {
        protected Mod()
        {
        }

        protected Mod(string modDirectory)
        {
            ModDirectory = modDirectory;
            if (!Exists())
                throw new ModExceptions("This Mod does not exists");
        }

        /// <summary>
        /// Returns the full Path of the Mods Root Directory
        /// </summary>
        public string ModDirectory { get; }

        /// <summary>
        /// Returns the name of the Mod
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Checks whether a mod exists
        /// </summary>
        /// <returns></returns>
        public bool Exists() => File.Exists(ModDirectory + @"\XML\Gameobjectfiles.xml");

        /// <summary>
        /// Searches in the current Directory for the presence of the mod and returns a new Mod Object
        /// A mod is found if the Gameobjectfile exists.
        /// </summary>
        /// <returns>Returns a new instance of the Mod</returns>
        public abstract Mod FindMod();
    }
}