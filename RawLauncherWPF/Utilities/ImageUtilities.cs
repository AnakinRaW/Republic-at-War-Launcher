using System;
using System.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RawLauncherWPF.Utilities
{
    class ImageUtilities
    {
        public static ImageSource GetImageSourceFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new NoNullAllowedException(nameof(path));
            return new BitmapImage(new Uri(@"pack://application:,,,/RawLauncherWPF;component/" + path));
        }
    }
}
