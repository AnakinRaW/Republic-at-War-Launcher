using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using RawLauncherWPF.ExtensionClasses;
using RawLauncherWPF.Utilities;
using static RawLauncherWPF.Configuration.Config;

namespace RawLauncherWPF.UI
{
    /// <summary>
    /// Interaktionslogik für BetaLogin.xaml
    /// </summary>
    public partial class BetaLogin
    {

        private bool Successfull;

        public BetaLogin()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (BetaUsers.FirstOrDefault(x => x.Value == UserName.Text.ToMd5()).Key == null ||
                Password.Password.ToMd5() != BetaPassword)
            {
                MessageProvider.Show("User-Name or Password was wrong");
                Successfull = false;
                return;
            }
            Successfull = true;
            Close();

        }

        private void BetaLogin_OnClosed(object sender, EventArgs e)
        {
            if (!Successfull)
                Process.GetCurrentProcess().Kill();
        }
    }
}
