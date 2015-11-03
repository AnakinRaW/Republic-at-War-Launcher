using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RawLauncherWPF.Utilities
{
    public class IsGreaterVersionColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values[0] is Version && values[1] is Version && (Version) values[0] < (Version) values[1]
                ? Brushes.Red
                : Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
