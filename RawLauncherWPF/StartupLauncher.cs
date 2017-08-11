using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.Localization;
using RawLauncherWPF.ResourceExtractor;
using static RawLauncher.Framework.Utilities.MessageProvider;
using BetaLogin = RawLauncher.Framework.UI.BetaLogin;
using LauncherApp = RawLauncher.Framework.Launcher.LauncherApp;

namespace RawLauncherWPF
{
    /// <summary>
    /// This class is the Entrypoint for the Launcher. 
    /// It performs Pre and Postlaunch tasks. 
    /// This ought not to interact with the launcher. I made this class static to make this clear
    /// 
    /// The Preparation includes extracting the needed resoures and other stuff that might not have something to do with the launcher.
    /// </summary>
    public static class StartupLauncher
    {
        [STAThread]
        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            try
            {
                ExtractLirbaries();
                //TODO: Activate
                //CheckBeta();
                SetUpLanguage();
                CheckRunning();
                CreateShortcut();

                var fu = new FrameworkUpdater();
                fu.UpdateIfNewVersionExists();

                var tu = new ThemeUpdater();
                tu.UpdateIfNewVersionExists();

                var laucher = new LauncherApp();
                laucher.InitializeComponent();
                laucher.Run();
            }
            catch (Exception e)
            {
                Show(e.GetType().FullName + "\r\n" + e.Message + "\r\n" + e.InnerException?.Message + "\r\n" + e.TargetSite);
            }

        }

        private static void CreateShortcut()
        {
            var link = (NativeMethods.NativeMethods.IShellLink) new NativeMethods.NativeMethods.ShellLink();

            link.SetDescription("Open the Republic at War Launcher");
            link.SetPath(Assembly.GetExecutingAssembly().Location);
            link.SetWorkingDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var file = (IPersistFile)link;
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            file.Save(Path.Combine(desktopPath, "Republic at War.lnk"), false);
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
                    Config.CurrentLanguage = new German();
                    break;
                case "es":
                    Config.CurrentLanguage = new Spanish();
                    break;
                default:
                    Config.CurrentLanguage = new English();
                    break;
            }
        }

        /// <summary>
        /// Extracts embadded DLL Libraries. 
        /// </summary>
        private static void ExtractLirbaries()
        {
            var audioExtractor = new ResourceExtractor.ResourceExtractor("Libraries");
            try
            {
                audioExtractor.ExtractFilesIfRequired(Directory.GetCurrentDirectory(),
                    new[]
                    {
                        "RawLauncher.Framework.dll", "RawLauncher.Theme.dll"
                    });
            }
            catch (ResourceExtractorException exception)
            {
                MessageBox.Show("Error" + exception.Message);
                Environment.Exit(0);
            }
        }

        private static bool IsApplicationAlreadyRunning()
        {
            return Process.GetProcesses().Count(p => p.ProcessName.Contains(Assembly.GetExecutingAssembly().FullName.Split(',')[0]) && !p.Modules[0].FileName.Contains("vshost")) > 1;
        }

    }
}