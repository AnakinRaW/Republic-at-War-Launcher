using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Microsoft.Win32;
using ModernApplicationFramework.Input.Command;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.Defreezer;
using RawLauncher.Framework.Launcher;
using RawLauncher.Framework.Localization;
using RawLauncher.Framework.Mods;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Controls
{
    public partial class AboutWindow
    {

        public ICommand DefreezeCommand => new Command(DefreezeAsync);

        public AboutWindow()
        {
            InitializeComponent();
            VersionNumber.Content = Assembly.GetExecutingAssembly().GetName().Version;
            ComboBox.SelectedIndex = Config.CurrentLanguage is German ? 1 : 0;
        }

        private void CloseWindow_CanExec(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CloseWindow_Exec(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox.SelectedIndex)
            {
                case 1:
                    Config.CurrentLanguage = new German();
                    break;
                case 2:
                    Config.CurrentLanguage = new Spanish();
                    break;
                default:
                    Config.CurrentLanguage = new English();
                    break;
            }
            Config.CurrentLanguage.Reload();
        }

        private static async void DefreezeAsync()
        {
            var launcher = IoC.Get<LauncherModel>();
            var initDir = launcher.CurrentMod == null ||
                          launcher.CurrentMod is DummyMod
                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                : launcher.BaseGame.SaveGameDirectory;

            var oFd = new OpenFileDialog
            {
                InitialDirectory = initDir,
                Filter = "Savegame Files (*.sav; *.PetroglyphFoCSave) | *.sav; *.PetroglyphFoCSave",
                Title = "Select a Savegame"
            };
            if (oFd.ShowDialog() != true)
                return;
            SaveGame saveGame;
            if (Path.GetExtension(oFd.FileName) == ".sav")
                saveGame = new RetailSaveGame(oFd.FileName);
            else
                saveGame = new SteamSaveGame(oFd.FileName);
            var d = new Defreezer.Defreezer(saveGame);
            await Task.Run(() => d.DefreezeSaveGame());
            MessageProvider.Show("Done");
        }
    }
}
