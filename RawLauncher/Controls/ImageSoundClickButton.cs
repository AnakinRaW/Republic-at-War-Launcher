using ModernApplicationFramework.Controls.Buttons;
using RawLauncher.Framework.Utilities;

namespace RawLauncher.Framework.Controls
{
    public class ImageSoundClickButton : ImageButton
    {
        protected override void OnClick()
        {
            AudioHelper.PlayAudio(AudioHelper.Audio.ButtonPress);
            base.OnClick();
        }
    }
}
