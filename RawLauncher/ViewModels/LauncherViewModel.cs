using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.Helpers;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Server;
using RawLauncher.Framework.UI;
using RawLauncher.Framework.Utilities;
using LauncherApp = RawLauncher.Framework.Launcher.LauncherApp;

namespace RawLauncher.Framework.ViewModels
{
    public sealed class LauncherViewModel : ViewModelBase, ILauncherViewModel
    {
        private readonly LauncherApp _launcher;
        private IMod _currentMod;
        private string _downloadDir;
        private IGame _eawGame;
        private IGame _baseGame;
        private IHostServer _hostServer;
        private string _restoreDir;
        private IServer _sessionServer;

        public LauncherViewModel(LauncherApp launcher)
        {
            _launcher = launcher;
            SetUpData();
        }

        public static IMod CurrentModStatic { get; private set; }
        public static IGame EawStatic { get; private set; }
        public static IGame FocStatic { get; private set; }
        public static IHostServer HostServerStatic { get; private set; }
        public static string RestoreDownloadDirStatic { get; private set; }
        public static string UpdateDownloadDirStatic { get; private set; }

        /// <summary>
        /// Tells if the raw.txt exists
        /// </summary>
        public bool FastLaunchFileExists => File.Exists(Configuration.Config.RaWAppDataPath + Configuration.Config.FastLaunchFileName);

        /// <summary>
        /// Contains the mod that shall be used for this launcher instance
        /// </summary>
        public IMod CurrentMod
        {
            get => _currentMod;
            set
            {
                if (value ==null)
                    return;
                if (Equals(value, _currentMod))
                    return;
                _currentMod = value;
                CurrentModStatic = value;
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
                EawStatic = value;
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
                FocStatic = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Contains the HostServer used for this launcher instance
        /// </summary>
        public IHostServer HostServer
        {
            get => _hostServer;
            set
            {
                if (value == null)
                    return;
                if (Equals(value, _hostServer))
                    return;
                _hostServer = value;
                HostServerStatic = value;
                OnPropertyChanged();
            }
        }

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
                RestoreDownloadDirStatic = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Contains the Session Server used for this launcher instance
        /// </summary>
        public IServer SessionServer
        {
            get => _sessionServer;
            set
            {
                if (Equals(value, _sessionServer))
                    return;
                _sessionServer = value;
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
                UpdateDownloadDirStatic = value;
                OnPropertyChanged();
            }
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

        public bool NewVersionAvailable() => CurrentMod?.Version < VersionUtilities.GetLatestModVersion();

        internal void ShowMainWindow(object index = null)
        {
            _launcher.MainWindow = new MainWindow(this);
            _launcher.MainWindow.Show();
            var a = (MainWindowViewModel)_launcher.MainWindow.DataContext;
            if (index != null)
                a.ShowPane(index);
            a.InstalledVersion = CurrentMod == null ? new Version("1.0") : CurrentMod.Version;
            a.LatestVersion = VersionUtilities.GetLatestModVersion();
        }

        internal void HideMainWindow()
        {
            _launcher.MainWindow.Hide();
        }

        /// <summary>
        /// Sets required Folders for Launcher
        /// </summary>
        private void InitDirectories()
        {
            if (!Directory.Exists(Configuration.Config.RaWAppDataPath))
                Directory.CreateDirectory(Configuration.Config.RaWAppDataPath);
            RestoreDownloadDir = Path.Combine(Configuration.Config.RaWAppDataPath);
            UpdateDownloadDir = Path.Combine(Configuration.Config.RaWAppDataPath);
        }

        /// <summary>
        /// Initialized both Games EaW and FoC
        /// Check for Steam usage first and create if found.
        /// If BaseGame NOT found -> Exit with Message
        /// If EaW NOT found -> Continue with Message of limited use. 
        /// </summary>
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

        /// <summary>
        /// Initialized the Mods. 
        /// If NOT found -> exit launcher
        /// </summary>
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

        /// <summary>
        /// Creates a new Server-Object
        /// </summary>
        private void InitServer()
        {
            HostServer  = new HostServer(Configuration.Config.ServerUrl);
            SessionServer = new SessionServer(Configuration.Config.SessionServerUrl);        
        }

        /// <summary>
        /// Initilizes important Data used by the Mod.
        /// </summary>
        private void SetUpData()
        {
            if (!InitGames())
                return;
            InitMod();
            InitDirectories();
            InitServer();
        }


        private static bool AskToUpdate()
        {
            var result =
                        MessageProvider.Show(MessageProvider.GetMessage("VersionUtilitiesAskForUpdate", VersionUtilities.GetLatestModVersion()),
                            "Republic at War", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            return result == MessageBoxResult.Yes;
        }

        #region Commands

        /// <summary>
        /// Performs a fast Launch
        /// </summary>
        public Command FastLaunchCommand => new Command(FastLaunch);

        private async void FastLaunch()
        {
            if (NativeMethods.NativeMethods.ComputerHasInternetConnection())
                if (NewVersionAvailable() && AskToUpdate())
                {
                    await DeleteFastLaunchFileCommand.Execute();
                    ShowMainWindow(4);
                    return;
                }
                if (PlayHelper.Play(BaseGame, CurrentMod))
                    ShowMainWindow(1);
        }

        /// <summary>
        /// Perform Normal launch
        /// </summary>
        public Command NormalLaunchCommand => new Command(NormalLaunch);

        private async void NormalLaunch()
        {         
           ShowMainWindow();
            if (NativeMethods.NativeMethods.ComputerHasInternetConnection() && CurrentMod != null && NewVersionAvailable())
                 await Task.Run(() => MessageProvider.Show(MessageProvider.GetMessage("LauncherInfoNewVersion", VersionUtilities.GetLatestModVersion()), "Republic at War", MessageBoxButton.OK, MessageBoxImage.Information));
        }

        public Command CreateFastLaunchFileCommand => new Command(CreateFastLaunchFile);

        private void CreateFastLaunchFile()
        {
            if (FastLaunchFileExists)
                return;
            try
            {
                File.WriteAllText(Configuration.Config.RaWAppDataPath + Configuration.Config.FastLaunchFileName,
                    CurrentMod.Version + "\r\n" + DateTime.Now.ToShortDateString() + "\r\n" + 
                    "Press and hold SHIFT while starting the launcher to access it again.") ;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public Command DeleteFastLaunchFileCommand => new Command(DeleteFastLaunchFile);

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

        #endregion
    }
}