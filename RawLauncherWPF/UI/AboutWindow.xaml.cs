using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RawLauncherWPF.Localization;
using static RawLauncherWPF.Configuration.Config;

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

            if (CurrentLanguage is German)
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
            CurrentLanguage = ComboBox.SelectedIndex == 1 ? (Language) new German() : new English();  
            CurrentLanguage.Reload();   
        }
    }
}
