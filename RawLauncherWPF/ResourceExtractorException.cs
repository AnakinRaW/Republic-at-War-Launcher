using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawLauncherWPF
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
