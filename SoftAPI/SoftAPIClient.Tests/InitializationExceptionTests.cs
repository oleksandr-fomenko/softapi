using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

            var result = DeserializeFromBytes(bytes);
            Assert.AreEqual(Message, result.Message);
            Assert.Null(result.InnerException);
        }

        private static byte[] SerializeToBytes(InitializationException e)
        {
            using var stream = new MemoryStream();
            new BinaryFormatter().Serialize(stream, e);
            return stream.GetBuffer();
        }

        private static InitializationException DeserializeFromBytes(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            return (InitializationException)new BinaryFormatter().Deserialize(stream);
        }
    }
}
