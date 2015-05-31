namespace RawLauncherWPF.Mods
{
    public interface IMod
    {
        /// <summary>
        /// Returns the full Path of the Mods Root Directory
        /// </summary>
        string ModDirectory { get; }

        /// <summary>
        /// Returns the name of the Mod
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Checks whether a mod exists
        /// </summary>
        /// <returns></returns>
        bool Exists();

        /// <summary>
        /// Read the Version from a Mod. Sets the Version of a mod
        ///
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// Searches in the current Directory for the presence of the mod and returns a new Mod Object
        /// A mod is found if the Gameobjectfile exists.
        /// </summary>
        /// <returns>Returns a new instance of the Mod</returns>
        IMod FindMod();
    }
}