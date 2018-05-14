using System;
using System.Globalization;
using System.Windows.Data;

namespace RawLauncher.Framework.Utilities.Converters
{
    public class BoolToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (bool) value)
                return 1;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
