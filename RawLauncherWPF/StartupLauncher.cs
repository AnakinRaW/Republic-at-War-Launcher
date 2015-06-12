using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using RawLauncherWPF.Games;
using RawLauncherWPF.Launcher;
using RawLauncherWPF.Models;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Server;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.Configuration.Config;
using static RawLauncherWPF.Models.LauncherModel;

namespace RawLauncherWPF
{
    /// <summary>
    /// This class is the Entrypoint for the Laucher. 
    /// It performs Pre and Postlaunch tasks. 
    /// This ought not to interact with the launcher. I made this class static to make this clear
    /// 
    /// The Preparation includes extracting the need resoures as well as decalring the mod, the two games and the dater miner for later usage.
    /// 
    /// TODO: Move all to a Launcher Object with Model.
    /// TODO: Leave Extracting Stuff in Main() Method.
    /// TODO: Make this Run for Steam also by using existing code as much as possible. Serverty is very low at the moment. If programming properly it should be easy to add afterwards.
    /// </summary>
    public static class StartupLauncher
    {
        public static void CleanUp()
        {
            //TODO: If there is something to clean up put it here.
            //TODO: Decide if in a catch block we shall perform a Clean-up
        }

        [STAThread]
        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            Prepare();
            PreStart();
            Start();
            CleanUp();
        }

        /// <summary>
        /// This Method contains some actions that shall be performed after the launcher is ready to launch but before showing up
        /// Can Close Application after this compleded tasks.
        /// Runs CleanUp on Exit
        /// TODO: Use Event instead
        /// TODO: Move SilentUpdateMod Call here 
        /// </summary>
        private static void PreStart()
        {
            // If "RaW.txt" does exists AND Shift is NOT pressed -> Show UpdateScreen and Run afterwards
            // Else Run MainWindow (which inits the the Update View which checks for update on creation)
            if (LauncherData.QuietLaunchFileExists && Keyboard.Modifiers != ModifierKeys.Shift)
            {

            }
            try
            {
                LauncherData.HostServer.CheckForUpdate(LauncherData.CurrentMod.Version);
            }
            catch (ServerException)
            {
            }
        }

        public static void Prepare()
        {
            new LauncherModel();
            ExtractLirbaries();
            ExtractAudio();
            SetupData();
        }

        public static void Start()
        {
            var app = new LauncherApp() {MainWindow = new MainWindow()};
            app.InitializeComponent();
            app.Run();
        }

        /// <summary>
        /// Extracts embedded audio files.
        /// </summary>
        private static void ExtractAudio()
        {
            var audioExtractor = new ResourceExtractor("Audio");
            try
            {
                audioExtractor.ExtractFilesIfRequired(Directory.GetCurrentDirectory() + @"\LecSetup",
                    new[]
                    {
                        "Play.WAV", "ButtonPress.WAV", "LauncherStartup.wav", "Checkbox.WAV", "QuitPress.WAV",
                        "MouseHover.WAV"
                    });
            }
            catch (ResourceExtractorException exception)
            {
                MessageBox.Show("Something went wrong when initializing the Launcher\n\n" + exception.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Extracts embadded DLL Libraries. 
        /// </summary>
        private static void ExtractLirbaries()
        {
            var audioExtractor = new ResourceExtractor("Libraries");
            try
            {
                audioExtractor.ExtractFilesIfRequired(Directory.GetCurrentDirectory(),
                    new[]
                    {
                        "Ionic.Zip.dll", "NAudio.dll", "ModernApplicationFramework.dll"
                    });
            }
            catch (ResourceExtractorException exception)
            {
                MessageBox.Show("Something went wrong when initializing the Launcher\n\n" + exception.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Initialized both Games EaW and FoC
        /// Check for Steam usage first and create if found.
        /// If Foc NOT found -> Exit with Message
        /// If EaW NOT found -> Continue with Message of limited use. 
        /// </summary>
        private static void InitGames()
        {
            try
            {
                var eaw = new Eaw().FindGame();
                LauncherData.SetEawGame(eaw);
                var foc = new Foc().FindGame();
                LauncherData.SetFocGame(foc);
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
        private static void InitMod()
        {
            try
            {
                var republicAtWar = new RaW().FindMod();
                LauncherData.SetCurrentMod(republicAtWar);
            }
            catch (ModExceptions e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Initilizes important Data used by the Mod.
        /// </summary>
        private static void SetupData()
        {     
            InitGames();
            InitMod();
            InitDirectories();
            InitServer();
        }

        /// <summary>
        /// Creates a new Server-Object
        /// </summary>
        private static void InitServer()
        {
            LauncherData.SetHostServer(new HostServer(ServerUrl));
        }

        /// <summary>
        /// Sets required Folders for Launcher
        /// </summary>
        private static void InitDirectories()
        {
            LauncherData.SetRestoreDownloadDir(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RaW_Modding_Team\");
            LauncherData.SetUpdateDownloadDir(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RaW_Modding_Team\");
        }
    }
}