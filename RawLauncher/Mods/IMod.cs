using System;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.ViewModels;

namespace RawLauncher.Framework.Mods
{
    public interface IMod
    {
        /// <summary>
        /// Returns the Name of the Folder, where the mod is stored
        /// </summary>
        string FolderName { get; }

        /// <summary>
        /// Returns the full Path of the Mods Root Directory
        /// </summary>
        string ModDirectory { get; }

        /// <summary>
        /// Returns the name of the Mod
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Read the Version from a Mod. Sets the Version of a mod
        ///
        /// </summary>
        Version Version { get; set; }

        /// <summary>
        /// Get the current installed language
        /// </summary>
        LanguageTypes InstalledLanguage { get; }

        /// <summary>
        /// Identifies whether the mod is a Steam Workshop instance
        /// </summary>
        bool WorkshopMod { get; }

        /// <summary>
        /// Checks whether a mod exists
        /// </summary>
        /// <returns></returns>
        bool Exists();

        /// <summary>
        /// Searches in the current Directory for the presence of the mod and returns a new Mod Object
        /// A mod is found if the Gameobjectfile exists.
        /// </summary>
        /// <returns>Returns a new instance of the Mod</returns>
        IMod FindMod(IGame baseGame);

        /// <summary>
        /// Prepares the mod for starting
        /// </summary>
        void PrepareStart(IGame game);

        /// <summary>
        /// Cleans stuff up after the game was palyed
        /// </summary>
        void CleanUpAferGame(IGame game);

        void Delete();
    }
}