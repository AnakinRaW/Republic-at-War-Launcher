using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RawLauncher.Framework.Hash
{
    public class HashProvider
    {
        public string GetFileHash(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(nameof(filePath));
            var md5 = new MD5CryptoServiceProvider();
            var fileReader = File.OpenRead(filePath);

            using (fileReader)
            {
                var md5Hash = md5.ComputeHash(fileReader);
                fileReader.Close();
                return Trim(md5Hash);
            }       
        }

        public string GetStringHash(string stringValue)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(stringValue);
            var md5Hash = md5.ComputeHash(inputBytes);
            return Trim(md5Hash);
        }

        public string GetDirectoryHash(string directory)
        {
            MerkleTree = string.Empty;
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException(nameof(directory));
            var files = Directory.GetFiles(directory);
            if (files.Length == 0)
                return "noHash";   
            foreach (var file in files)
            {
                MerkleTree += GetFileHash(file) + "\r\n";
            }
            return GetStringHash(MerkleTree);
        }


        private string Trim(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        private string MerkleTree { get; set; }

    }
}
