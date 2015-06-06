using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Windows;
using RawLauncherWPF.Games;
using RawLauncherWPF.Launcher;
using RawLauncherWPF.Mods;
using RawLauncherWPF.Server;
using RawLauncherWPF.Utilities;
using MainWindow = RawLauncherWPF.UI.MainWindow;

namespace RawLauncherWPF
{
    /// <summary>
    /// This class is the Entrypoint for the Laucher. 
    /// It performs Pre and Postlaunch tasks. 
    /// This ought not to interact with the launcher. I made this class static to make this clear
    /// 
    /// The Preparation includes extracting the need resoures as well as decalring the mod, the two games and the dater miner for later usage.
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
            Start();
            CleanUp();
        }

        public static void Prepare()
        {
            new LauncherDataMiner();
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

        private static void InitGames()
        {
            try
            {
                var eaw = new Eaw().FindGame();
                LauncherDataMiner.DataMiner.SetEawGame(eaw);
                var foc = new Foc().FindGame();
                LauncherDataMiner.DataMiner.SetFocGame(foc);
            }
            catch (GameExceptions e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }
        }

        private static void InitMod()
        {
            try
            {
                var republicAtWar = new RaW().FindMod();
                LauncherDataMiner.DataMiner.SetCurrentMod(republicAtWar);
            }
            catch (ModExceptions e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(0);
            }
        }

        private static void SetupData()
        {     
            InitGames();
            InitMod();
            InitDirectories();
            InitServer();
        }

        private static void InitServer()
        {
            LauncherDataMiner.DataMiner.SetHostServer(new HostServer(LauncherDataMiner.ServerUrl));
        }

        private static void InitDirectories()
        {
            LauncherDataMiner.DataMiner.SetRestoreDownloadDir(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RaW_Modding_Team\");
            LauncherDataMiner.DataMiner.SetUpdateDownloadDir(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\RaW_Modding_Team\");
        }
    }
}