using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawLauncherWPF.Localization
{
    class German : ILangueProvider 
    {
        public Dictionary<string, string> StringTable { get; }

        public German()
        {
            StringTable = new Dictionary<string, string>();
        }

        public string GetStringByKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}
