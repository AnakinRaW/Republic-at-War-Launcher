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
            var waveReader = new WaveFileReader(Directory.GetCurrentDirectory() + @"\LecSetup\" + file + ".wav");
            var waveOut = new WaveOut();
            var output = waveOut;
            output.Init(waveReader);
            output.Play();
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
