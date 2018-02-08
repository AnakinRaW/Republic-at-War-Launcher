using System;
using RawLauncher.Framework.Screens.CheckScreen;
using RawLauncher.Framework.Screens.PlayScreen;

namespace RawLauncher.Framework.Shell
{
    public partial class MainWindowView
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        public void ActivateTab(Type type)
        {
            if (type == typeof(IPlayScreen))
            {
                PlayTab.IsChecked = true;
                return;
            }

            if (type == typeof(ICheckScreen))
            {
                CheckTab.IsChecked = true;
                return;
            }
        }
    }
}
