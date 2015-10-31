using System.Windows.Media;

namespace RawLauncherWPF.Utilities
{
    public static class IndicatorImagesHelper
    {
        public enum IndicatorColor
        {
            Blue,
            Red,
            Yellow,
            Green,
            Gray
        }

        public static ImageSource SetColor(IndicatorColor color)
        {
            switch (color)
            {
                case IndicatorColor.Blue:
                    return ImageUtilities.GetImageSourceFromPath("Resources/Visual/Check/BlueIndicator.png");
                case IndicatorColor.Red:
                    return ImageUtilities.GetImageSourceFromPath("Resources/Visual/Check/RedIndicator.png");
                case IndicatorColor.Yellow:
                    return ImageUtilities.GetImageSourceFromPath("Resources/Visual/Check/YellowIndicator.png");
                case IndicatorColor.Green:
                    return ImageUtilities.GetImageSourceFromPath("Resources/Visual/Check/GreenIndicator.png");
                default:
                    return ImageUtilities.GetImageSourceFromPath("Resources/Visual/Check/GrayIndicator.png");
            }
        }
    }
}
