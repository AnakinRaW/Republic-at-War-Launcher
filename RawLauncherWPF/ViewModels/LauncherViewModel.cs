using System;
using System.IO;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.ViewModels;
using RawLauncherWPF.Games;
using RawLauncherWPF.Launcher;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Server;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.NativeMethods.NativeMethods;

namespace RawLauncherWPF.ViewModels
{
    public sealed class LauncherViewModel : ViewModelBase, ILauncherViewModel
    {
        private readonly LauncherApp _launcher;

        private IMod _currentMod;
        private IGame _eawGame;
        private IGame _focGame;
        private IServer _sessionServer;
        private IHostServer _hostServer;
        private string _downloadDir;
        private string _restoreDir;

        public LauncherViewModel(LauncherApp launcher)
        {
            _launcher = launcher;
            SetUpData();
        }

        public static IHostServer HostServerStatic { get; private set; }
        public static IGame EawStatic { get; private set; }
        public static IGame FocStatic { get; private set; }
        public static IMod CurrentModStatic { get; private set; }

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
        /// Contains a game, which should be Foc
        /// </summary>
        public IGame Foc
        {
            get { return _focGame; }
            set
            {
                if (value == null)
                    return;
                if (Equals(value, _focGame))
                    return;
                _focGame = value;
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
        /// If Foc NOT found -> Exit with Message
        /// If EaW NOT found -> Continue with Message of limited use. 
        /// </summary>
        private void InitGames()
        {
            try
            {
                var eaw = new Eaw().FindGame();
                Eaw = eaw;
                var foc = new Foc().FindGame();
                Foc = foc;
            }
            catch (GameExceptions e)
            {
                MessageProvider.Show(e.Message);
                Environment.Exit(0);
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
            catch (ModExceptions e)
            {
                MessageProvider.Show(e.Message);
                Environment.Exit(0);
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

        #region Commands

        /// <summary>
        /// Runs a special View of the Update Search procedure for the Fast-Launch
        /// </summary>
        public Command FastLaunchUpdateSearchCommand => new Command(FastLaunchUpdateSearch);

        private void FastLaunchUpdateSearch()
        {
            // TODO: Implement
            if (!ComputerHasInternetConnection())
                return;
            MessageProvider.Show("Should check for Update");
        }

        /// <summary>
        /// Starts the current setted mod
        /// </summary>
        public Command StartModCommand => new Command(StartMod);

        private void StartMod()
        {
            Foc.PlayGame(CurrentMod);
        }

        /// <summary>
        /// Sets up a new Mainwindow and Shows it
        /// </summary>
        public Command ShowMainWindowCommand => new Command(ShowMainWindow);

        private void ShowMainWindow()
        {
            _launcher.MainWindow = new MainWindow(this);
            _launcher.MainWindow.Show();
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