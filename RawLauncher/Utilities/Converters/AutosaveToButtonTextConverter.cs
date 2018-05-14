using System;
using System.Globalization;
using System.Windows.Data;

namespace RawLauncher.Framework.Utilities.Converters
{
    internal class AutosaveToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is bool flag))
                return string.Empty;
            return flag ? MessageProvider.GetMessage("EnableAutosave") : MessageProvider.GetMessage("DisableAutosave");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
