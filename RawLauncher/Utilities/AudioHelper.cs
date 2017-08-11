using System;
using System.IO;
using NAudio.Wave;

namespace RawLauncher.Framework.Utilities
{
    public static class AudioHelper
    {
        /// <summary>
        /// Plays audio from File as DirectAudio by using enum
        /// </summary>
        /// <param name="file"></param>
        public static void PlayAudio(Enum file)
        {
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
