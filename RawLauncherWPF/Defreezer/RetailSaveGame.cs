using System;
using System.IO;
using System.Text;

namespace RawLauncherWPF.Defreezer
{
    public sealed class RetailSaveGame : SaveGame
    {
        public RetailSaveGame(string filePath) : base(filePath)
        {
            if (Path.GetExtension(filePath) != ".sav")
                throw new FileFormatException();
        }

        public override string Name
        {
            get
            {
                if (ByteArray == null)
                    throw new NullReferenceException();
                var reader = new BinaryReader(File.Open(FilePath, FileMode.Open));
                reader.BaseStream.Position = 37;
                var name = Encoding.Unicode.GetString(reader.ReadBytes(500));
                reader.Close();
                return name;
            }
        }
    }
}