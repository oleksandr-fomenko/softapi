using System;
using System.Runtime.Serialization;

namespace SoftAPIClient.Core.Exceptions
{
    [Serializable]
    public sealed class InitializationException : Exception
    {
        public InitializationException(string message) : base(message)
        {
        }

        private InitializationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
