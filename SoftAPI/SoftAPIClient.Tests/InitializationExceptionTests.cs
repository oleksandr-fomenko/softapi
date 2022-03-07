using NUnit.Framework;
using SoftAPIClient.Core.Exceptions;

namespace SoftAPIClient.Tests
{
    public class InitializationExceptionTests : AbstractTest
    {
        [Test]
        public void VerifyInitializationExceptionMessage()
        {
            const string Message = "message";
            var exception = new InitializationException(Message);
            Assert.AreEqual(Message, exception.Message);
            var bytes = SerializeToBytes(exception);
            Assert.True(bytes.Length > 0);

            var result = DeserializeFromBytes<InitializationException>(bytes);
            Assert.AreEqual(Message, result.Message);
            Assert.Null(result.InnerException);
        }
    }
}
