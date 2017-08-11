using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RawLauncher.Framework.Configuration;
using RawLauncher.Framework.Localization;

namespace RawLauncher.Framework.UI
{
    /// <summary>
    /// Interaktionslogik für AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow
    {

        public AboutWindow()
        {
            InitializeComponent();
            VersionNumber.Content = Assembly.GetExecutingAssembly().GetName().Version;

            if (Config.CurrentLanguage is German)
                ComboBox.SelectedIndex = 1;
            else ComboBox.SelectedIndex = 0;
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
    }
}
