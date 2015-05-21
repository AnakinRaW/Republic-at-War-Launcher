using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using ModernApplicationFramework.Core.Events;

namespace RawLauncherWPF
{
    /// <summary>
    /// Interaktionslogik für "LauncherApp.xaml"
    /// </summary>
    public partial class LauncherApp
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var wnd = new MainWindow();
            wnd.Show();
        }
    }
}
