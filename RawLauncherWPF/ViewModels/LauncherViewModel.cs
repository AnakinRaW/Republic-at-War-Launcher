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
        private LauncherApp _launcher;

        public LauncherViewModel(LauncherApp launcher)
        {
            _launcher = launcher;
            SetUpData();
        }

        public bool QuietLaunchFileExists => File.Exists(Directory.GetCurrentDirectory() + @"\raw.txt");
        public IMod CurrentMod { get; private set; }
        public IGame Eaw { get; private set; }
        public IGame Foc { get; private set; }
        public IHostServer HostServer { get; private set; }
        public string RestoreDownloadDir { get; private set; }
        public string UpdateDownloadDir { get; private set; }

        public void SetCurrentMod(IMod mod)
        {
            if (mod == null)
                throw new NullReferenceException();
            CurrentMod = mod;
        }

        public void SetEawGame(IGame game)
        {
            if (game == null)
                throw new NullReferenceException();
            Eaw = game;
        }

        public void SetFocGame(IGame game)
        {
            if (game == null)
                throw new NullReferenceException(nameof(game));
            Foc = game;
        }

        public void SetHostServer(IHostServer server)
        {
            if (server == null)
                throw new NullReferenceException(nameof(server));
            HostServer = server;
        }

        public void SetRestoreDownloadDir(string path)
        {
            if (path == null)
                throw new NullReferenceException(nameof(path));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            RestoreDownloadDir = path;
        }

        public void SetUpdateDownloadDir(string path)
        {
            if (path == null)
                throw new NullReferenceException(nameof(path));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            UpdateDownloadDir = path;
        }

        /// <summary>
        /// Sets required Folders for Launcher
        /// </summary>
        private void InitDirectories()
        {
            SetRestoreDownloadDir(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RaW_Modding_Team\");
            SetUpdateDownloadDir(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RaW_Modding_Team\");
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
                SetEawGame(eaw);
                var foc = new Foc().FindGame();
                SetFocGame(foc);
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
                SetCurrentMod(republicAtWar);
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
            SetHostServer(new HostServer(Configuration.Config.ServerUrl));
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
            _launcher.MainWindow = new MainWindow();
            _launcher.MainWindow.Show();
        }

        #endregion
    }
}