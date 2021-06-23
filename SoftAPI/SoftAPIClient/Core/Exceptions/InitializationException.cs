using System;
using System.Runtime.Serialization;

namespace SoftAPIClient.Core.Exceptions
{
    [Serializable]
    public class InitializationException : Exception
    {
        public InitializationException(string message) : base(message)
        {
        }

        protected InitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
