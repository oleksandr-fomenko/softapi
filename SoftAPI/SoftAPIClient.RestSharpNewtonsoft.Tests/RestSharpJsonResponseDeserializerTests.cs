using NUnit.Framework;

namespace SoftAPIClient.RestSharpNewtonsoft.Tests
{
    public class RestSharpJsonResponseDeserializerTests : AbstractTest
    {

        [Test]
        public void WhenIncorrectStringIsProvidedJsonReaderExceptionIsThrown()
        {
            var restSharpJsonResponseDeserializer = new RestSharpJsonResponseDeserializer();

            var ex = Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => restSharpJsonResponseDeserializer.Convert<ResponseTests.UserJsonDto>("incorrect"));
            Assert.AreEqual("Unexpected character encountered while parsing value: i. Path '', line 0, position 0.", ex.Message);
        }

    }
}
