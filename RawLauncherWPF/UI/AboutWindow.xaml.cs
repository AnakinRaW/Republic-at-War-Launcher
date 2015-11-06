using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RawLauncherWPF.UI
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

            ComboBox.SelectedIndex = StartupLauncher.DisplayCulture.TwoLetterISOLanguageName == "de" ? 1 : 0;
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
            StartupLauncher.DisplayCulture = CultureInfo.CreateSpecificCulture(ComboBox.SelectedIndex == 0 ? "en" : "de");
        }
    }
}
