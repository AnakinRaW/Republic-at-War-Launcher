using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using RawLauncher.Framework.Launcher;

namespace RawLauncher.Framework.Utilities.Converters
{
    internal class IsTestersBuildToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var launcher = IoC.Get<LauncherModel>();

            if (launcher.UseDevHostServer)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    internal class TestGoalsStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var launcher = IoC.Get<LauncherModel>();
            return $"{launcher.CurrentMod?.Version.ToFullString()} test goals";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}