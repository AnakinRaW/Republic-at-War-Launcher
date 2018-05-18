using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using RawLauncher.Framework.Annotations;
using RawLauncher.Framework.Hash;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Games
{
    public sealed class SteamGame : AbstractFocGame, INotifyPropertyChanged
    {
        private bool _autosaveEnabled;
        public const string GameconstantsUpdateHash = "b0818f73031b7150a839bb83e7aa6187";

        protected override string GameExeFileName => "StarwarsG.exe";

        protected override int DefaultXmlFileCount => 1;

        public override string Name => "Forces of Corruption (Steam)";

        public string AutosaveFilePath => Path.Combine(SaveGameDirectory, "[AutoSave].PetroglyphFoCSave");

        public override string SaveGameDirectory
        {
            get
            {
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    @"Saved Games\Petroglyph\Empire At War - Forces of Corruption\Save\");
                return !Directory.Exists(folder) ? string.Empty : folder;
            }
        }

        public bool AutosaveEnabled
        {
            get => _autosaveEnabled;
            set
            {
                if (value == _autosaveEnabled)
                    return;
                ChangeAutosaveEnabledStatus(value);
                _autosaveEnabled = value;
                OnPropertyChanged();
            }
        }

        public SteamGame(string gameDirectory) : base(gameDirectory)
        {
            _autosaveEnabled = !FileUtilities.IsFileReadOnly(AutosaveFilePath);
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


            if (!Steam.IsSteamRunning()) 
                Steam.StartSteam();


            if (mod.Version > Version.Parse("1.2.0.1"))
                FileShuffler.ShuffleFiles(mod.ModDirectory + @"\Data\UnitNames\");

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

        public void SwitchAutosaveEnabledStatus()
        {
            AutosaveEnabled = !_autosaveEnabled;
        }

        private void ChangeAutosaveEnabledStatus(bool value)
        {
            if (value)
                FileUtilities.MakeFileWriteable(AutosaveFilePath);
            else
                FileUtilities.MakeFileReadOnly(AutosaveFilePath);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}