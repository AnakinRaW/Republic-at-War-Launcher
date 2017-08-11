using RawLauncher.Framework.Mods;

namespace RawLauncher.Framework.Games
{
    public interface IGame
    {
        /// <summary>
        /// Returns the full Path of the Games Root Directory
        /// </summary>
        string GameDirectory { get; }

        /// <summary>
        /// Return the full Path of the SaveGame Directory
        /// </summary>
        string SaveGameDirectory { get; }

        /// <summary>
        /// Returns the name of the Game
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Contains Data of the Process
        /// </summary>
        GameProcessData GameProcessData { get; }

        /// <summary>
        /// Checks whether a mod exists
        /// </summary>
        /// <returns></returns>
        bool Exists();

        /// <summary>
        /// Searches from the current Directory for the presence of the Game and returns a new Game Object
        /// A game is found if the the .exe exists.
        /// </summary>
        /// <returns>Returns a new instance of the Game</returns>
        IGame FindGame();

        /// <summary>
        /// Plays the default game
        /// </summary>
        void PlayGame();

        /// <summary>
        /// Plays the game with the mod
        /// </summary>
        /// <param name="mod"></param>
        void PlayGame(IMod mod);

        /// <summary>
        /// Patches the Game
        /// </summary>
        /// <returns></returns>
        bool Patch();

        /// <summary>
        /// Checks if the patch is installed
        /// </summary>
        /// <returns></returns>
        bool IsPatched();

        /// <summary>
        /// Deletes a Mod by it's name
        /// </summary>
        /// <param name="name"></param>
        void DeleteMod(string name);

        /// <summary>
        /// Removes any additional contnet from Rood Data folder.
        /// </summary>
        void ClearDataFolder();

        void BackUpAiFiles();

        void ResotreAiFiles();
    }
}