using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using RawLauncher.Framework.Games;
using RawLauncher.Framework.Launcher;

namespace RawLauncher.Framework.Utilities.Converters
{
    internal class SteamToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var launcher = IoC.Get<LauncherModel>();
            if (launcher?.BaseGame == null)
                return Visibility.Collapsed;
            return launcher.BaseGame is SteamGame ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
