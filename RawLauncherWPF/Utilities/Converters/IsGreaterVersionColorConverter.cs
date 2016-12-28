using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RawLauncherWPF.Utilities.Converters
{
    public class IsGreaterVersionColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            var normal = Application.Current.Resources["VersionNormalForeground"] as SolidColorBrush;
            var outdated = Application.Current.Resources["VersionOutdatedForeground"] as SolidColorBrush;

            return values[0] is Version && values[1] is Version && (Version) values[0] < (Version) values[1]
                ? outdated
                : normal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
