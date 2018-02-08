using System;

namespace RawLauncher.Framework.Server
{
    public class ServerException : Exception
    {
        public ServerException()
        {
        }

        public ServerException(string messsage): base (messsage)
        {
        }

        public ServerException(string message, Exception inner) : base (message, inner)
        {

        }
    }
}
