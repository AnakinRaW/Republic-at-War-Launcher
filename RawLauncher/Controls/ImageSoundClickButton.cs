using ModernApplicationFramework.Controls.Buttons;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Controls
{
    public class ImageSoundClickButton : ImageButton
    {
        protected override void OnClick()
        {
            AudioPlayer.PlayAudio(AudioPlayer.Audio.ButtonPress);
            base.OnClick();
        }
    }
}
