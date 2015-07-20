using System;
using System.IO;
using System.Windows;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.ViewModels;
using RawLauncherWPF.Games;
using RawLauncherWPF.Launcher;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Server;
using RawLauncherWPF.UI;

namespace RawLauncherWPF.ViewModels
{
    public sealed class LauncherViewModel : ViewModelBase
    {
        private readonly LauncherApp _launcher;

        private IMod _currentMod;
        private IGame _eawGame;
        private IGame _focGame;
        private IHostServer _hostServer;
        private string _downloadDir;
        private string _restoreDir;

        public LauncherViewModel(LauncherApp launcher)
        {
            _launcher = launcher;
            SetUpData();
        }

        /// <summary>
        /// Tells if the raw.txt exists
        /// </summary>
        public bool QuietLaunchFileExists => File.Exists(Directory.GetCurrentDirectory() + @"\raw.txt");

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
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Sets required Folders for Launcher
        /// </summary>
        private void InitDirectories()
        {
            RestoreDownloadDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                 @"\RaW_Modding_Team\";
            RestoreDownloadDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                 @"\RaW_Modding_Team\";
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
                MessageBox.Show(e.Message);
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
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Creates a new Server-Object
        /// </summary>
        private void InitServer()
        {
            HostServer  = new HostServer(Configuration.Config.ServerUrl);
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
            MessageBox.Show("Should check for Update");
        }

        /// <summary>
        /// Starts the current setted mod
        /// </summary>
        public Command StartModCommand => new Command(StartMod);

        private void StartMod()
        {
            // TODO: Implement
            MessageBox.Show("Should start Mod");
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

        #endregion
    }
}