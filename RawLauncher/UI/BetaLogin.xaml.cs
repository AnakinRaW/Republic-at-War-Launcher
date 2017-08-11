using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.ExtensionClasses;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.UI
{
    /// <summary>
    /// Interaktionslogik für BetaLogin.xaml
    /// </summary>
    public partial class BetaLogin
    {

        private bool _successfull;

        public BetaLogin()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (UserName.Text != Environment.UserName ||Config.BetaUsers.FirstOrDefault(x => x.Value == UserName.Text.ToMd5()).Key == null ||
                Password.Password.ToMd5() != Config.BetaPassword)
            {
                MessageProvider.Show("User-Name or Password was wrong");
                _successfull = false;
                return;
            }
            _successfull = true;
            Close();

        }

        private void BetaLogin_OnClosed(object sender, EventArgs e)
        {
            if (!_successfull)
                Process.GetCurrentProcess().Kill();
        }
    }
}
