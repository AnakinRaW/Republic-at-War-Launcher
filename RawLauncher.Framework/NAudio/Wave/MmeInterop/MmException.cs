using System;

namespace RawLauncher.Framework.NAudio.Wave.MmeInterop
{
    public class MmException : Exception
    {

        public MmException(MmResult result, string function)
            : base(ErrorMessage(result, function))
        {
        }


        private static string ErrorMessage(MmResult result, string function)
        {
            return $"{result} calling {function}";
        }
        
        public static void Try(MmResult result, string function)
        {
            if (result != MmResult.NoError)
                throw new MmException(result, function);
        }
    }
}
