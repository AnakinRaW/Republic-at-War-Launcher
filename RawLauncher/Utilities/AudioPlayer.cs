using System;
using NAudio.Wave;
using RawLauncher.Framework.Properties;

namespace RawLauncher.Framework.Utilities
{
    public static class AudioPlayer
    {
        /// <summary>
        /// Plays audio from File as DirectAudio by using enum
        /// </summary>
        /// <param name="file"></param>
        public static void PlayAudio(Enum file)
        {
            if (Settings.Default.SoundDisabled)
                return;
            try
            {
                var stream = Properties.Resources.ResourceManager.GetStream(file.ToString());
                var waveReader = new WaveFileReader(stream);
                var output = new WaveOut();
                output.Init(waveReader);
                output.Play();
            }
            catch (Exception)
            {
                //Ignored
            }
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
