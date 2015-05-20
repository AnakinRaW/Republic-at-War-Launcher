using System;
using System.Windows;

namespace RawLauncherWPF
{
    public class Launcher
    {
        [STAThread]
        [System.Diagnostics.DebuggerNonUserCodeAttribute]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            var launcher = new Launcher();
            launcher.StartLauncher();
        }


        public void StartLauncher()
        {
            MessageBox.Show("Test");
            var main = new App();
            main.InitializeComponent();
            main.Run();
        }
    }
}
