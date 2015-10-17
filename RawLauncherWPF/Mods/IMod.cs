using System;
using RawLauncherWPF.ViewModels;

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
        Version Version { get; set; }

        /// <summary>
        /// Searches in the current Directory for the presence of the mod and returns a new Mod Object
        /// A mod is found if the Gameobjectfile exists.
        /// </summary>
        /// <returns>Returns a new instance of the Mod</returns>
        IMod FindMod();

        /// <summary>
        /// Tests if the Mods folder structure matches an expected pattern
        /// </summary>
        bool HasCorrectFolderStructure { get; }

        /// <summary>
        /// Returns the Value needed to lauch the mod with command arguments
        /// </summary>
        string LaunchArgumentPath { get; }
    }
}