using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawLauncherWPF.Localization
{
    class English : ILangueProvider
    {
        public Dictionary<string, string> StringTable { get; }
        public string GetStringByKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}
