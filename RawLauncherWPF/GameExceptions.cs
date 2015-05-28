using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawLauncherWPF
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
