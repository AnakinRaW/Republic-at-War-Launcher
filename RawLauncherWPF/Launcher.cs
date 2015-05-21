using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace RawLauncherWPF
{
    public class Launcher
    {
        [STAThread]
        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {        
            var launcher = new Launcher();
            launcher.Prepare();
            launcher.Start();
        }

        public void Prepare()
        {
            ExtractLirbaries();
            ExtractAudio();
        }

        public void Start()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
            //At this point there can not be any more code interacting with the LauncherUI as the MainWindow can not be run in a different Thread.
            //Clean-Up code however can be placed here
            CleanUp();
        }

        public void CleanUp()
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
