using System;

namespace RawLauncher.Framework.Utilities
{
    [Flags]
    public enum LanguageTypes
    {
        None = 1,
        Dutch = 2,
        English = 4,
        French = 8,
        German = 16,
        Italian = 32,
        Russian = 64,
        Serbian = 128,
        Spanish = 265,
        Swedish = 512,
        Ukrainian = 1024,
        Custom = 2048
    }
}
