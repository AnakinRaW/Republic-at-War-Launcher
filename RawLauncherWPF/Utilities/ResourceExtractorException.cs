using System;

namespace RawLauncherWPF.Utilities
{
    internal class ResourceExtractorException : Exception
    {
        public ResourceExtractorException()
        {
        }

        public ResourceExtractorException(string messsage): base (messsage)
        {  
        }

        public ResourceExtractorException(string message, Exception inner) : base (message, inner)
        {
            
        }
    }
}
