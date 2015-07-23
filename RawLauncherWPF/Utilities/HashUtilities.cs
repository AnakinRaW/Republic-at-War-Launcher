using System;
using System.IO;
using System.Security.Cryptography;

namespace RawLauncherWPF.Utilities
{
    public static class HashUtilities
    {
        public static string GetMd5Hash(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var filereader = File.OpenRead(input);
            var md5Hash = md5.ComputeHash(filereader);
            var hash = BitConverter.ToString(md5Hash).Replace("-", "").ToLower();
            filereader.Close();
            return hash;
        }
    }
}
