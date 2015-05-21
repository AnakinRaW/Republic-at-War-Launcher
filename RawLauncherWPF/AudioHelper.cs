using System;
using System.IO;
using NAudio.Wave;

namespace RawLauncherWPF
{
    public static class AudioHelper
    {

        public static void PlayAudio(Enum file)
        {
            var audio = new AudioFileReader(Directory.GetCurrentDirectory() + @"\LecSetup\" + file + ".wav");
            IWavePlayer player = new DirectSoundOut();
            player.Init(audio);
            player.Play();
        }

        public enum Audio
        {
            ButtonPress,
            Checkbox,
            Play,
            QuitPress, 
            LauncherStartup,
            MouseHover,
        }
    }
}
