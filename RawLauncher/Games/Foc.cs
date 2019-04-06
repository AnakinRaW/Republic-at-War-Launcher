using System;
using System.Diagnostics;
using System.IO;
using RawLauncher.Framework.Hash;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Utilities;
using RawLauncher.Framework.Versioning;

namespace RawLauncher.Framework.Games
{
    public class Foc : AbstractFocGame
    {
        public const string GameconstantsUpdateHash = "b0818f73031b7150a839bb83e7aa6187";
        public const string GraphicdetailsUpdateHash = "4d7e140887fc1dd52f47790a6e20b5c5";

        protected override string GameExeFileName => "swfoc.exe";

        protected override int DefaultXmlFileCount => 2;

        public override string Name => "Forces of Corruption";

        public override string SaveGameDirectory
        {
            get
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Petroglyph\Empire At War - Forces of Corruption\Save\");
                if (!Directory.Exists(folder))
                    return "";
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Petroglyph\Empire At War - Forces of Corruption\Save\");
            }
        }

        public Foc() { }

        public Foc(string gameDirectory) : base(gameDirectory)
        {
        }

        public override bool IsPatched()
        {
            if (!File.Exists(GameDirectory + @"\Data\XML\GAMECONSTANTS.XML") ||
                !File.Exists(GameDirectory + @"\Data\XML\GRAPHICDETAILS.XML"))
                return false;
            var hashProvider = new HashProvider();
            if (hashProvider.GetFileHash(GameDirectory + @"\Data\XML\GAMECONSTANTS.XML") != GameconstantsUpdateHash)
                return false;
            if (hashProvider.GetFileHash(GameDirectory + @"\Data\XML\GRAPHICDETAILS.XML") != GraphicdetailsUpdateHash)
                return false;
            return true;
        }

        public override bool Patch()
        {
            try
            {
                if (!Directory.Exists(GameDirectory + @"\Data\XML"))
                    Directory.CreateDirectory(GameDirectory + @"\Data\XML");

                if (File.Exists(GameDirectory + @"\Data\XML\GAMECONSTANTS.XML"))
                    File.Delete(GameDirectory + @"\Data\XML\GAMECONSTANTS.XML");
                if (File.Exists(GameDirectory + @"\Data\XML\GRAPHICDETAILS.XML"))
                    File.Delete(GameDirectory + @"\Data\XML\GRAPHICDETAILS.XML");

                File.WriteAllText(GameDirectory + @"\Data\XML\GAMECONSTANTS.XML", Properties.Resources.GAMECONSTANTS);
                File.WriteAllText(GameDirectory + @"\Data\XML\GRAPHICDETAILS.XML", Properties.Resources.GRAPHICDETAILS);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public override void PlayGame(IMod mod)
        {
            if (mod == null)
                return;
            if (!mod.Exists())
                throw new ModExceptions(MessageProvider.GetMessage("ExceptionGameModExist"));
            if (!mod.ModDirectory.StartsWith(GameDirectory))
                throw new ModExceptions(MessageProvider.GetMessage("ExceptionGameModCompatible"));

            if (mod.Version > ModVersion.Parse("1.2.0.1"))
                FileShuffler.ShuffleFiles(mod.ModDirectory + @"\Data\UnitNames\");

            var process = new Process
            {
                StartInfo =
                {
                    FileName = GameDirectory + @"\swfoc.exe",
                    Arguments = "MODPATH=" + "Mods/" + mod.FolderName,
                    WorkingDirectory = GameDirectory,
                    UseShellExecute = false
                }
            };
            try
            {
                GameStartHelper.StartGameProcess(process);
            }
            catch (Exception)
            {
                //ignored
            }
        }
    }
}