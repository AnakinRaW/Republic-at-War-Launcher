using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using RawLauncherWPF.Launcher;
using RawLauncherWPF.Utilities;

namespace RawLauncherWPF
{
    /// <summary>
    /// This class is the Entrypoint for the Laucher. 
    /// It performs Pre and Postlaunch tasks. 
    /// This ought not to interact with the launcher. I made this class static to make this clear
    /// 
    /// The Preparation includes extracting the needed resoures and other stuff that might not have something to do with the launcher.
    /// 
    /// TODO: Make this Run for Steam also by using existing code as much as possible. Serverty is very low at the moment. If programming properly it should be easy to add afterwards.
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
            ExtractLibraries();

            _launcher = new LauncherApp();
            _launcher.InitializeComponent();
            _launcher.Run();

            CleanUp();
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
                MessageProvider.Show("Something went wrong when initializing the Launcher\n\n" + exception.Message);
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
                MessageProvider.Show("Something went wrong when initializing the Launcher\n\n" + exception.Message);
                Environment.Exit(0);
            }
        }

    }
}