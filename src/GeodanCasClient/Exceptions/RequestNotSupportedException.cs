using System;

namespace Geodan.Cloud.Client.Core.Exceptions
{
    public class RequestNotSupportedException : Exception
    {
        private const string Msg = "The request you are trying to execute is currently not supported";
        public RequestNotSupportedException(string message = Msg)
            : base(message)
        {
        }

        public RequestNotSupportedException(Exception inner, string message = Msg)
            : base(message, inner)
        {
        }
    }
}
