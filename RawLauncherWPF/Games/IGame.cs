﻿namespace RawLauncherWPF.Games
{
    public interface IGame
    {
        /// <summary>
        /// Returns the full Path of the Games Root Directory
        /// </summary>
        string GameDirectory { get; }

        /// <summary>
        /// Returns the name of the Game
        /// </summary>
        string Name { get; }

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
    }
}