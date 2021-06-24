using System.Runtime.Serialization;
using NUnit.Framework;
using SoftAPIClient.Core.Exceptions;

namespace SoftAPIClient.Tests
{
    public class InitializationExceptionTests : InitializationException
    {
        public InitializationExceptionTests() : base(nameof(InitializationExceptionTests))
        {
        }

        public InitializationExceptionTests(string message) : base(message)
        {
        }

        protected InitializationExceptionTests(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        [Test]
        public void VerifyInitializationExceptionMessage()
        {
            Assert.AreEqual(nameof(InitializationExceptionTests), base.Message);
        }
    }
}
