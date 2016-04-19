using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using RawLauncherWPF.Launcher;
using RawLauncherWPF.Localization;
using RawLauncherWPF.UI;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.Configuration.Config;
using static RawLauncherWPF.Utilities.MessageProvider;

namespace RawLauncherWPF
{
    /// <summary>
    /// This class is the Entrypoint for the Laucher. 
    /// It performs Pre and Postlaunch tasks. 
    /// This ought not to interact with the launcher. I made this class static to make this clear
    /// 
    /// The Preparation includes extracting the needed resoures and other stuff that might not have something to do with the launcher.
    /// </summary>
    public static class StartupLauncher
    {
        private static LauncherApp _launcher;

        public static void CleanUp()
        {
           _launcher = null;
        }

        [STAThread]
        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            try
            {
                //TODO: Activate
                //CheckBeta();
                SetUpLanguage();
                CheckRunning();

                ExtractLibraries();
                _launcher = new LauncherApp();
                _launcher.InitializeComponent();
                _launcher.Run();

                CleanUp();
            }
            catch (Exception e)
            {
                Show(e.Message);
            }
            
        }

        private static void CheckRunning()
        {
            if (!IsApplicationAlreadyRunning())
                return;
            Show(GetMessage("ErrorAlreadyRunning"));
            Environment.Exit(0);
        }

        private static void CheckBeta()
        {
            new BetaLogin().ShowDialog();
        }

        private static void SetUpLanguage()
        {
            switch (CultureInfo.InstalledUICulture.TwoLetterISOLanguageName)
            {
                case "de":
                    CurrentLanguage = new German();
                    break;
                case "es":
                    CurrentLanguage = new Spanish();
                    break;
                default:
                    CurrentLanguage = new English();
                    break;
            }
        }


        public static void ExtractLibraries()
        {
            ExtractLirbaries();
            ExtractAudio();
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

                Show(GetMessage("ErrorInitLauncher", exception.Message));
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
                Show(GetMessage("ErrorInitLauncher", exception.Message));
                Environment.Exit(0);
            }
        }

        static bool IsApplicationAlreadyRunning()
        {
            return Process.GetProcesses().Count(p => p.ProcessName.Contains(Assembly.GetExecutingAssembly().FullName.Split(',')[0]) && !p.Modules[0].FileName.Contains("vshost")) > 1;
        }

    }
}