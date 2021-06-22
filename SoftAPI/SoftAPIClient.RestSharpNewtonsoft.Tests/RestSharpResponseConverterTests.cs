using System.Collections.Generic;
using Newtonsoft.Json;
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

        [Test]
        public void VerifyJsonBodyHandling()
        {
            var userObject = new ResponseTests.UserJsonDto
            {
                Age = 22,
                Name = "JsonLad"
            };
            var expectedParameters = new List<Parameter>
            {
                new Parameter("application/json", JsonConvert.SerializeObject(userObject), ParameterType.RequestBody),
                new JsonParameter("", userObject,"application/json"),
            };


            var request = new Request
            {
                Body = new KeyValuePair<BodyType, object>(BodyType.Json, userObject)
            };
            var restRequest = new RestRequest(Method.GET);

            var restSharpResponseConverter = new RestSharpResponseConverter();
            restSharpResponseConverter.HandleBody(request, restRequest);

            var actualParameters = restRequest.Parameters;
            Assert.AreEqual(expectedParameters, actualParameters);
        }

        [Test]
        public void VerifyXmlBodyHandling()
        {
            var userObject = new UserXmlDto
            {
                Age = 23,
                Name = "XmlLad"
            };

            var expectedParameters = new List<Parameter>
            {
                new XmlParameter("", userObject),
            };

            var request = new Request
            {
                Body = new KeyValuePair<BodyType, object>(BodyType.Xml, userObject)
            };
            var restRequest = new RestRequest(Method.GET);

            var restSharpResponseConverter = new RestSharpResponseConverter();
            restSharpResponseConverter.HandleBody(request, restRequest);

            Assert.IsNotNull(restRequest.XmlSerializer);
            Assert.IsInstanceOf<RestSharp.Serializers.DotNetXmlSerializer>(restRequest.XmlSerializer);
            Assert.AreEqual(DataFormat.Xml, restRequest.RequestFormat);
            var actualParameters = restRequest.Parameters;
            Assert.AreEqual(expectedParameters, actualParameters);
        }

    }
}
