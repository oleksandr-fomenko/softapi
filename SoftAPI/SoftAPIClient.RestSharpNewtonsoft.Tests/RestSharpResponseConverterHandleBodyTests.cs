using System.Collections.Generic;
using System.Xml.Serialization;
using NUnit.Framework;
using RestSharp;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.RestSharpNewtonsoft.Tests
{
    [TestFixture]
    public class RestSharpResponseConverterHandleBodyTests : RestSharpResponseConverter
    {
        [Test]
        public void VerifyIfNullBodyIsProvidedRestRequestHasNoBody()
        {
            var request = new Request();
            var restRequest = new RestRequest(Method.GET);

            HandleBody(request, restRequest);

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
                new Parameter("application/json", new RestSharpJsonSerializer().Convert(userObject), ParameterType.RequestBody),
                new JsonParameter("", userObject,"application/json"),
            };


            var request = new Request
            {
                Body = new KeyValuePair<BodyType, object>(BodyType.Json, userObject)
            };
            var restRequest = new RestRequest(Method.GET);

            HandleBody(request, restRequest);

            var actualParameters = restRequest.Parameters;
            Assert.AreEqual(expectedParameters, actualParameters);
        }

        [Test]
        public void VerifyJsonBodyHandling_WithBodyName()
        {
            var userObject = new ResponseTests.UserJsonDto
            {
                Age = 22,
                Name = "JsonLad"
            };
            var expectedParameters = new List<Parameter>
            {
                new Parameter("test_name", new RestSharpJsonSerializer().Convert(userObject),"application/json", ParameterType.RequestBody)
            };

            var request = new Request
            {
                BodyName = "test_name",
                Body = new KeyValuePair<BodyType, object>(BodyType.Json, userObject)
            };
            var restRequest = new RestRequest(Method.GET);

            HandleBody(request, restRequest);

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

            HandleBody(request, restRequest);

            Assert.IsNotNull(restRequest.XmlSerializer);
            Assert.IsInstanceOf<RestSharp.Serializers.DotNetXmlSerializer>(restRequest.XmlSerializer);
            Assert.AreEqual(DataFormat.Xml, restRequest.RequestFormat);
            var actualParameters = restRequest.Parameters;
            Assert.AreEqual(expectedParameters, actualParameters);
        }

        [Test]
        public void VerifyXmlBodyHandling_WithBodyName()
        {
            var userObject = new UserXmlDto
            {
                Age = 23,
                Name = "XmlLad"
            };
            var expectedParameters = new List<Parameter>
            {
                new Parameter("test_name", new RestSharp.Serializers.DotNetXmlSerializer().Serialize(userObject),"application/xml", ParameterType.RequestBody)
            };

            var request = new Request
            {
                BodyName = "test_name",
                Body = new KeyValuePair<BodyType, object>(BodyType.Xml, userObject)
            };
            var restRequest = new RestRequest(Method.GET);

            HandleBody(request, restRequest);

            var actualParameters = restRequest.Parameters;
            Assert.AreEqual(expectedParameters, actualParameters);
        }

        [Test]
        public void VerifyTextBodyHandling()
        {
            const string textBody = "TxtLad";

            var expectedParameters = new List<Parameter>
            {
                new Parameter("text/plain", textBody, ParameterType.RequestBody)
            };

            var request = new Request
            {
                Body = new KeyValuePair<BodyType, object>(BodyType.Text, textBody)
            };
            var restRequest = new RestRequest(Method.GET);

            HandleBody(request, restRequest);

            var actualParameters = restRequest.Parameters;
            Assert.AreEqual(expectedParameters, actualParameters);
        }

        [Test]
        public void VerifyTextBodyHandling_WithBodyName()
        {
            const string textBody = "TxtLad";

            var expectedParameters = new List<Parameter>
            {
                new Parameter("test_name", textBody, "text/plain" , ParameterType.RequestBody)
            };

            var request = new Request
            {
                BodyName = "test_name",
                Body = new KeyValuePair<BodyType, object>(BodyType.Text, textBody)
            };
            var restRequest = new RestRequest(Method.GET);

            HandleBody(request, restRequest);

            var actualParameters = restRequest.Parameters;
            Assert.AreEqual(expectedParameters, actualParameters);
        }
    }

    [XmlRoot(ElementName = "root")]
    public class UserXmlDto
    {
        [XmlElement(ElementName = "age")]
        public int Age { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
    }
}
