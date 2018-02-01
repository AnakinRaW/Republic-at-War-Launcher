using System;
using System.Diagnostics;
using System.IO;
using RawLauncher.Framework.Hash;
using RawLauncher.Framework.Helpers;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Games
{
    public sealed class SteamGame : AbstractFocGame
    {
        public const string GameconstantsUpdateHash = "b0818f73031b7150a839bb83e7aa6187";

        protected override string GameExeFileName => "StarwarsG.exe";

        protected override int DefaultXmlFileCount => 2;

        public override string Name => "Forces of Corruption (Steam)";

        public override string SaveGameDirectory
        {
            get
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    @"Saved Games\Petroglyph\Empire At War - Forces of Corruption\Save\");
                if (!Directory.Exists(folder))
                    return "";
                return folder;
            }
        }

        public SteamGame()
        {
        }

        public SteamGame(string gameDirectory) : base(gameDirectory)
        {
        }

        public override bool IsPatched()
        {
            if (!File.Exists(GameDirectory + @"\Data\XML\GAMECONSTANTS.XML") 
                //|| !File.Exists(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML")
                )
                return false;
            var hashProvider = new HashProvider();
            if (hashProvider.GetFileHash(GameDirectory + @"\Data\XML\GAMECONSTANTS.XML") != GameconstantsUpdateHash)
                return false;
            //if (hashProvider.GetFileHash(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML") != GraphicdetailsUpdateHash)
            //    return false;
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
                //File.WriteAllText(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML", Properties.Resources.GRAPHICDETAILS);
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
            if (!Exists())
                throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExistName", Name));

            FileShuffler.ShuffleFiles(mod.ModDirectory + @"\Data\XML\UnitNames\");


            string arguments;
            if (!mod.WorkshopMod)
                arguments = "MODPATH=" + "Mods/" + mod.FolderName;
            else
                arguments = "NOARTPROCESS IGNOREASSERTS STEAMMOD=" + mod.FolderName;

            var process = new Process
            {
                StartInfo =
                {
                    FileName = GameDirectory + @"\StarwarsG.exe",
                    Arguments = arguments,
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