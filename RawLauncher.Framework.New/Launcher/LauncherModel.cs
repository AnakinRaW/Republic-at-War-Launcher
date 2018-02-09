using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Server;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Launcher
{
    [Export(typeof(LauncherModel))]
    public class LauncherModel : ViewModelBase //, ILauncherModel
    {
        private string _restoreDir;
        private string _downloadDir;
        private IGame _eawGame;
        private IGame _baseGame;
        private IMod _currentMod;


        /// <summary>
        /// Tells if the raw.txt exists
        /// </summary>
        public bool FastLaunchFileExists =>
            File.Exists(Configuration.Config.RaWAppDataPath + Configuration.Config.FastLaunchFileName);

        /// <summary>
        /// Contains the HostServer used for this launcher instance
        /// </summary>
        public IHostServer HostServer { get; }

        /// <summary>
        /// Contains the current Restore Directory
        /// </summary>
        public string RestoreDownloadDir
        {
            get => _restoreDir;
            set
            {
                if (value == null)
                    return;
                if (Equals(value, _restoreDir))
                    return;
                _restoreDir = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Contains the Update Directory
        /// </summary>
        public string UpdateDownloadDir
        {
            get => _downloadDir;
            set
            {
                if (value == null)
                    return;
                if (Equals(value, _downloadDir))
                    return;
                _downloadDir = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Contains the mod that shall be used for this launcher instance
        /// </summary>
        public IMod CurrentMod
        {
            get => _currentMod;
            set
            {
                if (value == null)
                    return;
                if (Equals(value, _currentMod))
                    return;
                _currentMod = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Contains a game, which should be EaW
        /// </summary>
        public IGame Eaw
        {
            get => _eawGame;
            set
            {
                if (value == null)
                    return;
                if (Equals(value, _eawGame))
                    return;
                _eawGame = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Contains a game, which should be BaseGame
        /// </summary>
        public IGame BaseGame
        {
            get => _baseGame;
            set
            {
                if (value == null)
                    return;
                if (Equals(value, _baseGame))
                    return;
                _baseGame = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Performs a fast Launch
        /// </summary>
        public Command FastLaunchCommand => new Command(FastLaunch);

        /// <summary>
        /// Perform Normal launch
        /// </summary>
        public Command NormalLaunchCommand => new Command(NormalLaunch);

        public Command CreateFastLaunchFileCommand => new Command(CreateFastLaunchFile);

        public Command DeleteFastLaunchFileCommand => new Command(DeleteFastLaunchFile);

        [ImportingConstructor]
        public LauncherModel(IHostServer hostServer)
        {
            if (!InitGames())
                return;
            InitMod();
            InitDirectories();
            HostServer = hostServer;
        }

        public string GetRescueFilePath(string fileName, bool online, Version version = null)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            if (version == null)
                return GetRescueFilePath(fileName, online, CurrentMod.Version);
            if (online)
                return version + @"\RescueFiles\" + fileName;
            return RestoreDownloadDir + @"\RescueFiles\" + version + @"\" + fileName;
        }

        private void InitDirectories()
        {
            if (!Directory.Exists(Configuration.Config.RaWAppDataPath))
                Directory.CreateDirectory(Configuration.Config.RaWAppDataPath);
            RestoreDownloadDir = Path.Combine(Configuration.Config.RaWAppDataPath);
            UpdateDownloadDir = Path.Combine(Configuration.Config.RaWAppDataPath);
        }

        private bool InitGames()
        {
            try
            {
                var result = GameHelper.GetInstalledGameType(Directory.GetCurrentDirectory());

                if (result.Type == GameTypes.Disk)
                {
                    Eaw = new Eaw().FindGame();
                    BaseGame = new Foc(result.FocPath);
                }
                else if (result.Type == GameTypes.SteamGold)
                {
                    Eaw = new Eaw().FindGame();
                    BaseGame = new SteamGame(result.FocPath);
                }
                else if (result.Type == GameTypes.GoG)
                {
                    Eaw = new Eaw().FindGame();
                    BaseGame = new Foc(result.FocPath);
                }
            }
            catch (GameExceptions)
            {
                return false;
            }
            return true;
        }

        private void InitMod()
        {
            try
            {
                var republicAtWar = new RaW().FindMod(BaseGame);
                CurrentMod = republicAtWar;
            }
            catch (ModExceptions)
            {
                //Show(e.Message);
                //Environment.Exit(0);
            }
        }


        private void DeleteFastLaunchFile()
        {
            if (!FastLaunchFileExists)
                return;
            try
            {
                File.Delete(Configuration.Config.RaWAppDataPath + Configuration.Config.FastLaunchFileName);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        private void CreateFastLaunchFile()
        {
            if (FastLaunchFileExists)
                return;
            try
            {
                File.WriteAllText(Configuration.Config.RaWAppDataPath + Configuration.Config.FastLaunchFileName,
                    CurrentMod.Version + "\r\n" + DateTime.Now.ToShortDateString() + "\r\n" +
                    "Press and hold SHIFT while starting the launcher to access it again.");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async void NormalLaunch()
        {
            if (NativeMethods.NativeMethods.ComputerHasInternetConnection() && CurrentMod != null && NewVersionAvailable())
                await Task.Run(() => MessageProvider.Show(MessageProvider.GetMessage("LauncherInfoNewVersion", VersionUtilities.GetLatestModVersion()), "Republic at War", MessageBoxButton.OK, MessageBoxImage.Information));
        }

        private async void FastLaunch()
        {
            if (NativeMethods.NativeMethods.ComputerHasInternetConnection())
                if (NewVersionAvailable() && AskToUpdate())
                {
                    await DeleteFastLaunchFileCommand.Execute();
                    //TODO:
                    //ShowMainWindow(4);
                    return;
                }
            //if (PlayHelper.Play(BaseGame, CurrentMod))
            //    ShowMainWindow(1);
        }

        private bool NewVersionAvailable() => CurrentMod?.Version < VersionUtilities.GetLatestModVersion();

        private static bool AskToUpdate()
        {
            var result =
                MessageProvider.Show(MessageProvider.GetMessage("VersionUtilitiesAskForUpdate", VersionUtilities.GetLatestModVersion()),
                    "Republic at War", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            return result == MessageBoxResult.Yes;
        }
    }
}
