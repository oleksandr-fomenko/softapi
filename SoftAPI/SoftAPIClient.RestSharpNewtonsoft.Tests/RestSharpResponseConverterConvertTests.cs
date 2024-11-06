using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
using RestSharp;
using SoftAPIClient.MetaData;
using SoftAPIClient.Core;
using Moq;

namespace SoftAPIClient.RestSharpNewtonsoft.Tests
{
    [TestFixture]
    public class RestSharpResponseConverterConvertTests : RestSharpResponseConverter
    {
        private static readonly ResponseTests.UserJsonDto UserObject = new ResponseTests.UserJsonDto
        {
            Age = 23,
            Name = "JsonLad"
        };
        private static readonly string SerializedBody = new RestSharpJsonSerializer().Convert(UserObject);
        private const string Url = "http://localhost:8080";
        private static readonly Uri Uri = new Uri(Url);
        private const HttpStatusCode StatusCode = HttpStatusCode.OK;
        private const string Method = "POST";
        private const string ContentType = "application/json";
        private const string Path = "/path";
        private static readonly Dictionary<string, object> PathParameters = new Dictionary<string, object> { { "path_int_key", 1 }, { "path_string_key", "string" }, { "path_null_key", null } };
        private static readonly Dictionary<string, object> QueryParameters = new Dictionary<string, object> { { "query_int_key", 1 }, { "query_string_key", "string" }, { "query_null_key", null } };
        private static readonly Dictionary<string, object> FormDataParameters = new Dictionary<string, object> { { "formData_int_key", 1 }, { "formData_string_key", "string" } };
        private static readonly List<KeyValuePair<string, string>> Headers = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Content-Type", ContentType),
            new KeyValuePair<string, string>("x-api-key", null),
        };

        private static readonly List<Core.FileParameter> FileParameters = new List<Core.FileParameter>
        {
            new Core.FileParameter("testFile", new byte[] { 1, 2, 3 }, "test.jpeg", "image/jpeg")
        };
        private static readonly List<KeyValuePair<string, string>> Cookies = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Cookie1", "1"),
            new KeyValuePair<string, string>("Cookie2", null)
        };
        private static readonly KeyValuePair<BodyType, object> Body = new KeyValuePair<BodyType, object>(BodyType.Json, UserObject);
        private static readonly RestSharpJsonResponseDeserializer Deserializer = new RestSharpJsonResponseDeserializer();
        private static readonly DynamicRequestSettings DynamicRequestSettings = new DynamicRequestSettings
        {
            Encoder = s => s
        };
        private static readonly Exception exception = new Exception();

        private RestResponse _restResponse;

        [TestCaseSource(nameof(GetTestData))]
        public void VerifyConvertRequest(Tuple<RestResponse, Request, Response> input)
        {
            SetRestResponse(input.Item1);
            var expectedResponse = input.Item3;
            var actualResponse = Convert(() => input.Item2);

            Assert.AreEqual(expectedResponse.HttpStatusCode, actualResponse.HttpStatusCode);
            Assert.AreEqual(expectedResponse.ResponseUri, actualResponse.ResponseUri);
            Assert.AreEqual(expectedResponse.ContentType, actualResponse.ContentType);
            Assert.AreEqual(expectedResponse.OriginalRequest, actualResponse.OriginalRequest);
            Assert.AreEqual(expectedResponse.ResponseBodyString, actualResponse.ResponseBodyString);
            Assert.AreEqual(expectedResponse.Headers, actualResponse.Headers);
            Assert.AreEqual(expectedResponse.Cookies, actualResponse.Cookies);
            Assert.IsNotNull(actualResponse.Deserializer);
            Assert.IsInstanceOf<RestSharpJsonResponseDeserializer>(actualResponse.Deserializer);
        }

        [Test]
        public void VerifyGetRestResponse()
        {
            const HttpStatusCode httpStatusCode = HttpStatusCode.OK;
            var response = new Mock<IRestResponse>();
            response.Setup(_ => _.StatusCode).Returns(httpStatusCode);
            
            var mockIRestClient = new Mock<IRestClient>();
            mockIRestClient
                .Setup(x => x.Execute(It.IsAny<IRestRequest>()))
                .Returns(response.Object);

            var restResponse = base.GetRestResponse(mockIRestClient.Object, new RestRequest());

            Assert.AreEqual(httpStatusCode, restResponse.StatusCode);
        }

        protected override IRestResponse GetRestResponse(IRestClient client, RestRequest restRequest)
        {
            return _restResponse;
        }

        private static IEnumerable<Tuple<RestResponse, Request, Response>> GetTestData()
        {
            var headersRestRequest = new List<Parameter>
            {
                new Parameter("Content-Type", ContentType, ParameterType.HttpHeader),
                new Parameter("x-api-key", null, ParameterType.HttpHeader),
            };

            var cookiesRestRequest = new List<RestResponseCookie>
            {
                new RestResponseCookie
                {
                    Name = "Cookie1",
                    Value = "1"
                },
                new RestResponseCookie
                {
                    Name = "Cookie2",
                    Value = null
                }
            };

            var restResponse = new RestResponse
            {
                StatusCode = StatusCode,
                ResponseUri = Uri,
                ContentType = ContentType,
                ErrorException = exception,
                Content = SerializedBody
            };
            SetInternalProps(restResponse, headersRestRequest, cookiesRestRequest);
            var request = new Request
            {
                Url = Url,
                Method = Method,
                Path = Path,
                PathParameters = PathParameters,
                QueryParameters = QueryParameters,
                FormDataParameters = FormDataParameters,
                Headers = Headers,
                FileParameters = FileParameters,
                Body = Body,
                Deserializer = Deserializer,
                Settings = DynamicRequestSettings
            };
            var expectedResponse = new Response
            {
                HttpStatusCode = StatusCode,
                ResponseUri = Uri,
                Headers = Headers,
                Cookies = Cookies,
                ContentType = ContentType,
                OriginalRequest = request,
                OriginalResponse = restResponse,
                ResponseBodyString = SerializedBody,
                Exception = exception,
                Deserializer = Deserializer
            };

            yield return new Tuple<RestResponse, Request, Response>(restResponse, request, expectedResponse);


            restResponse = new RestResponse
            {
                StatusCode = StatusCode,
                ResponseUri = Uri,
                ContentType = ContentType,
                ErrorException = null,
                Content = null
            };
            SetInternalProps(restResponse);
            request = new Request
            {
                Url = Url,
                Method = Method,
                Path = Path,
                PathParameters = new Dictionary<string, object>(),
                QueryParameters = new Dictionary<string, object>(),
                FormDataParameters = new Dictionary<string, object>(),
                Headers = new List<KeyValuePair<string, string>>(),
                FileParameters = new List<Core.FileParameter>(),
                Body = default,
                Deserializer = null,
                Settings = null
            };
            expectedResponse = new Response
            {
                HttpStatusCode = StatusCode,
                ResponseUri = Uri,
                Headers = new List<KeyValuePair<string, string>>(),
                Cookies = new List<KeyValuePair<string, string>>(),
                ContentType = ContentType,
                OriginalRequest = request,
                OriginalResponse = restResponse,
                ResponseBodyString = string.Empty,
                Exception = null,
                Deserializer = null
            };

            yield return new Tuple<RestResponse, Request, Response>(restResponse, request, expectedResponse);
        }

        private void SetRestResponse(RestResponse restResponse)
        {
            _restResponse = restResponse;
        }

        private static void SetInternalProps(RestResponse restResponse, IList<Parameter> restHeaders = null, IList<RestResponseCookie> restCookies = null)
        {
            var type = typeof(RestResponse);
            if (restHeaders != null)
            {
                type.GetProperty("Headers")?.SetValue(restResponse, restHeaders);
            }
            if (restCookies != null)
            {
                type.GetProperty("Cookies")?.SetValue(restResponse, restCookies);
            }
        }

    }
}
