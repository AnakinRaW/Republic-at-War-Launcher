using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using ModernApplicationFramework.Basics.ViewModels;
using ModernApplicationFramework.CommandBase;
using RawLauncherWPF.Games;
using RawLauncherWPF.Launcher;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Server;
using RawLauncherWPF.UI;
using static RawLauncherWPF.NativeMethods.NativeMethods;
using static RawLauncherWPF.Utilities.MessageProvider;
using static RawLauncherWPF.Utilities.VersionUtilities;

namespace RawLauncherWPF.ViewModels
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
        public bool FastLaunchFileExists => File.Exists(Directory.GetCurrentDirectory() + @"\" + Configuration.Config.FastLaunchFileName);

        /// <summary>
        /// Contains the mod that shall be used for this launcher instance
        /// </summary>
        public IMod CurrentMod
        {
            get { return _currentMod; }
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
            get { return _eawGame; }
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
            get { return _baseGame; }
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
            get { return _hostServer; }
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
            get { return _restoreDir; }
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
            get { return _sessionServer; }
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
            get { return _downloadDir; }
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
                return @"RescueFiles\" + version + @"\" + fileName;
            return RestoreDownloadDir + @"\RescueFiles\" + version + @"\" + fileName;

        }

        public bool NewVersionAvailable() => CurrentMod?.Version < GetLatestModVersion();

        internal void ShowMainWindow(object index = null)
        {
            _launcher.MainWindow = new MainWindow(this);
            _launcher.MainWindow.Show();
            var a = (MainWindowViewModel)_launcher.MainWindow.DataContext;
            if (index != null)
                a.ShowPane(index);
            a.InstalledVersion = CurrentMod == null ? new Version("1.0") : CurrentMod.Version;
            a.LatestVersion = GetLatestModVersion();
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
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RaW_Modding_Team")))
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RaW_Modding_Team"));
            RestoreDownloadDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RaW_Modding_Team");
            UpdateDownloadDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RaW_Modding_Team");
        }

        /// <summary>
        /// Initialized both Games EaW and FoC
        /// Check for Steam usage first and create if found.
        /// If BaseGame NOT found -> Exit with Message
        /// If EaW NOT found -> Continue with Message of limited use. 
        /// </summary>
        private void InitGames()
        {
            try
            {
                if (GameHelper.GetInstalledGameType(Directory.GetCurrentDirectory()) == GameTypes.Disk)
                {
                    Eaw = new Eaw().FindGame();
                    BaseGame = new Foc().FindGame();
                }
                else if (GameHelper.GetInstalledGameType(Directory.GetCurrentDirectory()) == GameTypes.SteamGold)
                {
                    Eaw = new Eaw(Directory.GetParent(Directory.GetCurrentDirectory()) + @"\GameData\");
                    BaseGame = new SteamGame().FindGame();
                }
            }
            catch (GameExceptions)
            {
                //Show(e.Message);
                //Environment.Exit(0);
            }
        }

        /// <summary>
        /// Initialized the Mods. 
        /// If NOT found -> exit launcher
        /// </summary>
        private void InitMod()
        {
            try
            {
                var republicAtWar = new RaW().FindMod();
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
            InitGames();
            InitMod();
            InitDirectories();
            InitServer();
        }


        private bool AskToUpdate()
        {
            var result =
                        Show(GetMessage("VersionUtilitiesAskForUpdate", GetLatestModVersion()),
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
            if (ComputerHasInternetConnection())
                if (NewVersionAvailable() && AskToUpdate())
                {
                    await DeleteFastLaunchFileCommand.Execute();
                    ShowMainWindow(4);
                    return;
                }
            CurrentMod.PrepareStart(BaseGame);
            await Task.Run(() => BaseGame.PlayGame(CurrentMod));

            BaseGame.GameProcessData.PropertyChanged += GameProcessData_PropertyChanged;
        }

        private void GameProcessData_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(GameProcessData.IsProcessRunning))
                return;
            CurrentMod.CleanUpAferGame(BaseGame);
            BaseGame.GameProcessData.PropertyChanged -= GameProcessData_PropertyChanged;
            _launcher.Shutdown();
        }

        /// <summary>
        /// Perform Normal launch
        /// </summary>
        public Command NormalLaunchCommand => new Command(NormalLaunch);

        private async void NormalLaunch()
        {         
           ShowMainWindow();
            if (ComputerHasInternetConnection() && CurrentMod != null && NewVersionAvailable())
                 await Task.Run(() => Show(GetMessage("LauncherInfoNewVersion", GetLatestModVersion()), "Republic at War", MessageBoxButton.OK, MessageBoxImage.Information));
        }

        public Command CreateFastLaunchFileCommand => new Command(CreateFastLaunchFile);

        private void CreateFastLaunchFile()
        {
            if (FastLaunchFileExists)
                return;
            try
            {
                File.WriteAllText(Directory.GetCurrentDirectory() + @"\" + Configuration.Config.FastLaunchFileName,
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
                File.Delete(Directory.GetCurrentDirectory() + @"\" + Configuration.Config.FastLaunchFileName);
            }
            catch (Exception)
            {
                //ignored
            }
        }

        #endregion
    }
}