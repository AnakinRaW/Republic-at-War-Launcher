using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace RawLauncherWPF
{
    /// <summary>
    /// This class is the Entrypoint for the Laucher. 
    /// It performs Pre and Postlaunch tasks. 
    /// This ought not to interact with the launcher. I made this class static to make this clear
    /// </summary>
    public static class Launcher
    {
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
            ExtractLirbaries();
            ExtractAudio();
        }

        public static void Start()
        {
            var app = new LauncherApp();
            app.InitializeComponent();
            app.Run();
        }

        public static void CleanUp()
        {
            //TODO: If there is something to clean up put it here.
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
    }
}
