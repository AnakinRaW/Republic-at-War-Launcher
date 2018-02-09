using System;
using RawLauncher.Framework.Screens.CheckScreen;
using RawLauncher.Framework.Screens.LanguageScreen;
using RawLauncher.Framework.Screens.PlayScreen;
using RawLauncher.Framework.Screens.Restore;
using RawLauncher.Framework.Screens.UpdateScreen;

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
            if (type == typeof(ILanguageScreen))
            {
                LangTab.IsChecked = true;
                return;
            }
            if (type == typeof(IRestoreScreen))
            {
                RestoreTab.IsChecked = true;
            }
            if (type == typeof(IUpdateScreen))
            {
                UpdateTab.IsChecked = true;
            }
        }
    }
}
