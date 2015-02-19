using System;

namespace Geodan.Cloud.Client.Core.Exceptions
{
    public class CasNotAuthenticatedException : Exception
    {
        private const string Msg = "Not authenticated with cas";
        public CasNotAuthenticatedException(string message = Msg)
            : base(message)
        {
        }

        public CasNotAuthenticatedException(Exception inner, string message = Msg)
            : base(message, inner)
        {
        }
    }
}
