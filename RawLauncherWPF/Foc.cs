using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawLauncherWPF
{
    public class Foc : Game
    {
        public Foc()
        {
        }

        public Foc(string gameDirectory) : base(gameDirectory)
        {
        }

        public override string Name => "Forces of Corruption";

        public override Game FindGame()
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\swfoc.exe"))
                throw new GameExceptions(Name + " does not exists");
            return new Foc(Directory.GetCurrentDirectory());
        }
    }
}
