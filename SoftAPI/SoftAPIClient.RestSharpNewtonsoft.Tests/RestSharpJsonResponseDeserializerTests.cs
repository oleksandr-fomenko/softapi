using NUnit.Framework;

namespace SoftAPIClient.RestSharpNewtonsoft.Tests
{
    public class RestSharpJsonResponseDeserializerTests : AbstractTest
    {

        [Test]
        public void WhenIncorrectStringIsProvidedTheDefaultInstanceIsCreated()
        {
            var restSharpJsonResponseDeserializer = new RestSharpJsonResponseDeserializer();
            var result = restSharpJsonResponseDeserializer.Convert<ResponseTests.UserJsonDto>("incorrect");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<ResponseTests.UserJsonDto>(result);
        }

    }
}
