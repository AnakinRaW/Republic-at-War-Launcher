using ModernApplicationFramework.Controls.Buttons;
using RawLauncherWPF.Utilities;

namespace RawLauncherWPF.Controls
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
