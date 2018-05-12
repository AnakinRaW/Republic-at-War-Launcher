using System;

namespace RawLauncher.Framework.Games
{
    public class GameExceptions : Exception
    {
        public GameExceptions()
        {
        }

        public GameExceptions(string messsage): base (messsage)
        {
        }

        public GameExceptions(string message, Exception inner) : base (message, inner)
        {

        }
    }
}
