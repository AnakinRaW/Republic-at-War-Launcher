using System;

namespace RawLauncherWPF
{
    class ModExceptions : Exception
    {
        public ModExceptions()
        {
        }

        public ModExceptions(string messsage): base (messsage)
        {
        }

        public ModExceptions(string message, Exception inner) : base (message, inner)
        {

        }
    }
}
