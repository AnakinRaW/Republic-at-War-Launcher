using System.Data;

namespace RawLauncherWPF.Defreezer
{
    public static class SearchAlgorithm
    {
        /// <summary>
        /// Searches a Byte-Array for a Pattern and returns the position of the beginning of the pattern
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="pattern"></param>
        /// <param name="startPosition"></param>
        /// <returns></returns>
        public static int SearchKmp(byte[] byteArray, byte[] pattern, int startPosition)
        {
            if (byteArray == null || pattern == null || startPosition < 0)
                throw new NoNullAllowedException();

            var m = startPosition;
            var i = 0;
            var T = BuildKmpTable(pattern);

            while (m + i < byteArray.Length)
            {
                if (pattern[i] == byteArray[m + i])
                {
                    if (i == pattern.Length - 1)
                        return m;
                    i++;
                }
                else
                {
                    m = m + i - T[i];
                    i = T[i] > -1 ? T[i] : 0;
                }
            }
            return -1;
        }

        private static int[] BuildKmpTable(byte[] pattern)
        {
            var T = new int[pattern.Length];
            var pos = 2;
            var cnd = 0;
            T[0] = -1;
            T[1] = 0;
            while (pos < pattern.Length)
            {
                if (pattern[pos - 1] == pattern[cnd])
                {
                    cnd++;
                    T[pos] = cnd;
                    pos++;
                }
                else if (cnd > 0)
                    cnd = T[cnd];
                else
                {
                    T[pos] = 0;
                    pos++;
                }
            }
            return T;
        }
    }
}