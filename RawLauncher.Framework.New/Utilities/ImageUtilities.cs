using System;
using System.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RawLauncher.Framework.Utilities
{
    internal class ImageUtilities
    {
        public static ImageSource GetImageSourceFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new NoNullAllowedException(nameof(path));
            return new BitmapImage(new Uri(@"pack://application:,,,/RawLauncher.Theme;component/" + path));
        }
    }
}
