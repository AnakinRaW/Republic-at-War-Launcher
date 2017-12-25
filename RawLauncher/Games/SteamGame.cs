﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using RawLauncher.Framework.Hash;
using RawLauncher.Framework.Helpers;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Games
{
    public sealed class SteamGame : IGame
    {
        public const string GameconstantsUpdateHash = "b0818f73031b7150a839bb83e7aa6187";
        //public const string GraphicdetailsUpdateHash = "4d7e140887fc1dd52f47790a6e20b5c5";

        public SteamGame()
        {
        }

        public SteamGame(string gameDirectory)
        {
            GameDirectory = gameDirectory;
            if (!Exists())
                throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExist"));
            GameProcessData = new GameProcessData();
        }

        public void ClearDataFolder()
        {
            if (Directory.Exists(@"Data\CustomMaps"))
                FileUtilities.DeleteDirectory(@"Data\CustomMaps");
            if (Directory.Exists(@"Data\Scripts"))
                FileUtilities.DeleteDirectory(@"Data\Scripts");
            if (Directory.Exists(@"Data\XML"))
                FileUtilities.DeleteDirectory(@"Data\XML");
        }

        //public void BackUpAiFiles()
        //{
        //    ClearBackupFiles();
        //    if (Directory.Exists(@"Data\CustomMaps"))
        //        Directory.Move(@"Data\CustomMaps", @"Data\CustomMapsBackup");
        //    if (Directory.Exists(@"Data\Scripts"))
        //        Directory.Move(@"Data\Scripts", @"Data\ScriptsBackup");
        //    if (Directory.Exists(@"Data\Xml"))
        //        Directory.Move(@"Data\Xml", @"Data\XmlBackup");
        //}

        //public void ResotreAiFiles()
        //{
        //    ClearDataFolder();
        //    if (Directory.Exists(@"Data\CustomMapsBackup"))
        //        Directory.Move(@"Data\CustomMapsBackup", @"Data\CustomMaps");
        //    if (Directory.Exists(@"Data\ScriptsBackup"))
        //        Directory.Move(@"Data\ScriptsBackup", @"Data\Scripts");
        //    if (Directory.Exists(@"Data\XmlBackup"))
        //        Directory.Move(@"Data\XmlBackup", @"Data\Xml");
        //    ClearBackupFiles();
        //}

        //public void ClearBackupFiles()
        //{
        //    if (Directory.Exists(@"Data\CustomMapsBackup"))
        //        Directory.Delete(@"Data\CustomMapsBackup", true);
        //    if (Directory.Exists(@"Data\ScriptsBackup"))
        //        Directory.Delete(@"Data\ScriptsBackup", true);
        //    if (Directory.Exists(@"Data\XmlBackup"))
        //        Directory.Delete(@"Data\XmlBackup", true);
        //}

        public void DeleteMod(IMod mod)
        {
            if (mod == null)
            {
                MessageBox.Show("Republic at War was not found");
                return;
            }
            if (mod is DummyMod)
                return;
            ClearDataFolder();
            Patch();
            mod.Delete();
        }

        public GameProcessData GameProcessData { get; }

        public bool Exists() => File.Exists(GameDirectory + @"\StarwarsG.exe");

        public IGame FindGame()
        {
            throw new NotImplementedException();
            //if (!File.Exists(Directory.GetCurrentDirectory() + @"\StarwarsG.exe"))
            //    throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExistName", Name));
            //return new SteamGame(Directory.GetCurrentDirectory() + @"\");
        }

        public string GameDirectory { get; }

        public bool IsPatched()
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

        public string Name => "Forces of Corruption (Steam)";

        public bool Patch()
        {
            try
            {
                if (!Directory.Exists(GameDirectory + @"\Data\XML"))
                    Directory.CreateDirectory(GameDirectory + @"\Data\XML");

                if (File.Exists(GameDirectory + @"\Data\XML\GAMECONSTANTS.XML"))
                    File.Delete(GameDirectory + @"\Data\XML\GAMECONSTANTS.XML");
                if (File.Exists(GameDirectory + @"\Data\XML\GRAPHICDETAILS.XML"))
                    File.Delete(GameDirectory + @"\Data\XML\GRAPHICDETAILS.XML");

                File.WriteAllText(GameDirectory + @"Data\XML\GAMECONSTANTS.XML", Properties.Resources.GAMECONSTANTS);
                //File.WriteAllText(GameDirectory + @"Data\XML\GRAPHICDETAILS.XML", Properties.Resources.GRAPHICDETAILS);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public void PlayGame()
        {
            PlayGame(null);
        }

        public void PlayGame(IMod mod)
        {
            //var startInfo = new ProcessStartInfo
            //{
            //    FileName = Steam.SteamExePath,
            //};

            //if (mod == null)
            //    startInfo.Arguments = "-applaunch 32470 swfoc";
            //else
            //    startInfo.Arguments = "-applaunch 32470 swfoc MODPATH=" + mod.LaunchArgumentPath;

            //string str = Directory.GetParent(new DirectoryInfo(Directory.GetCurrentDirectory()).FullName).FullName;

            if (!Exists())
                throw new GameExceptions(MessageProvider.GetMessage("ExceptionGameExistName", Name));

            //File.Move(str + "\\runme.dat", str + "\\tmp.runme.dat.tmp");
            //File.Copy(str + "\\runm2.dat", str + "\\runme.dat");
            //Process.Start(startInfo);
            //Thread.Sleep(2000);
            //File.Delete(str + "\\runme.dat");
            //File.Move(str + "\\tmp.runme.dat.tmp", str + "\\runme.dat");

            FileShuffler.ShuffleFiles(mod.ModDirectory + @"\Data\XML\UnitNames\");


            string arguments = string.Empty;

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
                //GameProcessData.Process = process;
                //GameProcessData.StartProcess();
            }
            catch (Exception)
            {
                //ignored
            }


            //GameProcessData.Process = ProcessHelper.FindProcess("swfoc");
        }

        public string SaveGameDirectory
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
    }
}