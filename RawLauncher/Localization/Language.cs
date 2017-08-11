using System;
using System.Collections.Generic;

namespace RawLauncher.Framework.Localization
{
    public abstract class Language
    {
        protected abstract Dictionary<string, string> StringTable { get; }

        public string GetStringByKey(string messageId, params object[] args)
        {
            if (messageId == null)
                return string.Empty;
            try
            {
                var result = string.Format(StringTable[messageId], args);
                return result;
            }
            catch (Exception)
            {
                return "Text Create Error";
            }
        }

        protected abstract void FillTable();

        public abstract void Reload();
    }
}
