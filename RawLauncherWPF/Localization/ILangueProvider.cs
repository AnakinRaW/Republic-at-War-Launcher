using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawLauncherWPF.Localization
{
    interface ILangueProvider
    {
        Dictionary<string, string> StringTable { get; }

        string GetStringByKey(string key);
    }
}
