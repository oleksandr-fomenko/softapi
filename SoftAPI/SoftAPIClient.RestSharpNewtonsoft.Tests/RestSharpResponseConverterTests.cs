using NUnit.Framework;
using RestSharp;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.RestSharpNewtonsoft.Tests
{
    public class RestSharpResponseConverterTests : AbstractTest
    {
        [Test]
        public void VerifyIfNullBodyIsProvidedRestRequestHasNoBody()
        {
            var request = new Request();
            var restRequest = new RestRequest(Method.GET);

            var restSharpResponseConverter = new RestSharpResponseConverter();
            restSharpResponseConverter.HandleBody(request, restRequest);

            Assert.Null(restRequest.Body);
        }

    }
}
