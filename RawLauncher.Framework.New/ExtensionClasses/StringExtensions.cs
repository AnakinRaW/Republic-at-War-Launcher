using System.IO;
using RawLauncher.Framework.Hash;

namespace RawLauncher.Framework.ExtensionClasses
{
    public static class StringExtensions
    {
        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string ToMd5(this string @this)
        {
            return new HashProvider().GetStringHash(@this);
        }
    }
}
