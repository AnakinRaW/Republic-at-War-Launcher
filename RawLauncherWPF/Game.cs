using System.IO;

namespace RawLauncherWPF
{
    public abstract class Game
    {
        protected Game()
        {
        }

        protected Game(string gameDirectory)
        {
            GameDirectory = gameDirectory;
            if (!Exists())
                throw new ModExceptions("This Game does not exists");
        }

        /// <summary>
        /// Returns the full Path of the Games Root Directory
        /// </summary>
        public string GameDirectory { get; }

        /// <summary>
        /// Returns the name of the Game
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Checks whether a mod exists
        /// </summary>
        /// <returns></returns>
        public bool Exists() => Directory.Exists(GameDirectory);

        /// <summary>
        /// Searches from the current Directory for the presence of the Game and returns a new Game Object
        /// A game is found if the the .exe exists.
        /// </summary>
        /// <returns>Returns a new instance of the Game</returns>
        public abstract Game FindGame();
    }
}