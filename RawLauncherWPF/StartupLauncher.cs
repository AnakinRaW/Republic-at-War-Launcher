using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.Localization;
using RawLauncherWPF.ResourceExtractor;
using RawLauncherWPF.Updaters;
using static RawLauncher.Framework.Utilities.MessageProvider;
using BetaLogin = RawLauncher.Framework.UI.BetaLogin;
using LauncherApp = RawLauncher.Framework.Launcher.LauncherApp;
using MessageBox = System.Windows.MessageBox;

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
        public const string ServerUrl = "https://gitlab.com/Republic-at-War/Republic-At-War/raw/";


        public static Window Splash;

        [STAThread]
        [DebuggerNonUserCode]
        [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            try
            {        
                if (!Get46FromRegistry())
                    Environment.Exit(0);
                          
                ExtractLirbaries();


                if (!File.Exists(Directory.GetCurrentDirectory() + @"\" + "RawLauncherInfo.txt"))
                    SplashScreen.ShowSplashScreen();

                var fu = new FrameworkUpdater();
                fu.UpdateIfNewVersionExists();

                var tu = new ThemeUpdater();
                tu.UpdateIfNewVersionExists();

                //TODO: Activate
                //CheckBeta();
                SetUpLanguage();
                CheckRunning();
                CreateShortcut();

                try
                {
                    RunApplication();
                }
                finally
                {
                    if (SplashScreen.SplashScreenForm != null)
                        SplashScreen.SplashScreenForm.Dispose();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.GetType().FullName + "\r\n" + e.Message + "\r\n" + e.InnerException?.Message + "\r\n" + e.TargetSite
                    + "\r\n" + e.StackTrace);
            }
        }

	    private static void RunApplication()
	    {
		    var host = new LauncherHost(AppDomain.CurrentDomain);
		    host.BeforeLaunch += delegate
		    {
		        if (SplashScreen.SplashScreenForm == null)
		            return;
		        SplashScreen.SplashScreenForm.BeginInvoke(new MethodInvoker(SplashScreen.SplashScreenForm.Dispose));
		        SplashScreen.SplashScreenForm = null;
		    };	    
		    host.Run();
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

        private static bool Get46FromRegistry()
        {
            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey?.GetValue("Release") != null)
                {
                    if (CheckFor46DotVersion((int)ndpKey.GetValue("Release")))
                        return true;
                    MessageBox.Show("Required .NetFramework Version 4.6 was not found");
                    return false;
                }
                MessageBox.Show("Required .NetFramework Version 4.6 was not found");
                return false;
            }
        }

        private static bool CheckFor46DotVersion(int releaseKey)
        {
            return releaseKey >= 393295;
        }
    }

    public class LauncherHost
    {
        public event EventHandler BeforeLaunch;

        readonly CallHelper _helper;

        public LauncherHost(AppDomain appDomain)
	    {
		    _helper = (CallHelper) appDomain.CreateInstanceAndUnwrap(typeof(LauncherHost).Assembly.FullName, typeof(CallHelper).FullName);
		    _helper.InitCore(new CallbackHelper(this));
	    }

	    public void Run()
	    {
		    _helper.Run();
	    }
	    
        internal sealed class CallbackHelper : MarshalByRefObject
        {
	        private readonly LauncherHost _host;

	        public CallbackHelper(LauncherHost host)
	        {
		        _host = host;
	        }

	        internal void BeforeLaunch()
	        {
		        _host.BeforeLaunch?.Invoke(_host, EventArgs.Empty);
	        }
        }
    }

	internal sealed class CallHelper : MarshalByRefObject
	{
		private LauncherHost.CallbackHelper _callback;

	    private static LauncherApp _launcherApp;

        public void InitCore(LauncherHost.CallbackHelper callback)
        {
            _callback = callback;
            _launcherApp = new LauncherApp();
            _launcherApp.InitializeComponent();
        }

		public void Run()
		{
		    RunInternal();
		}

		void RunInternal()
		{
		    _callback.BeforeLaunch();
            _launcherApp.Run();
        }
	} 
}