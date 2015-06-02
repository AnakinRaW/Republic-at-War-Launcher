using System;
using System.IO;
using NAudio.Wave;

namespace RawLauncherWPF.Utilities
{
    public static class AudioHelper
    {
        /// <summary>
        /// Plays audio from File as DirectAudio by using enum
        /// </summary>
        /// <param name="file"></param>
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
