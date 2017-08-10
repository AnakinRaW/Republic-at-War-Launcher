namespace RawLauncher.Framework.NAudio.Utils
{
    public static class BufferHelpers
    {
        public static float[] Ensure(float[] buffer, int samplesRequired)
        {
            if (buffer == null || buffer.Length < samplesRequired)
            {
                buffer = new float[samplesRequired];
            }
            return buffer;
        }
    }
}
