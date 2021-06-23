using System;

namespace SoftAPIClient.Core.Exceptions
{
    public class InitializationException : Exception
    {
        public InitializationException(string message) : base(message)
        {
        }
    }
}
