using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Exceptions;
using SoftAPIClient.MetaData;
using System;
using System.Collections.Generic;
using System.Net;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.Implementations;

namespace SoftAPIClient.Tests
{
    public class RestClientTests : AbstractTest
    {
        [Test]
        public void VerifyInitializationExceptionWhenNoClientAttributeProvided()
        {
            var ex = Assert.Throws<InitializationException>(() => RestClient.Instance.GetService<IInterfaceWithoutClientAttribute>().Get("1").Invoke());
            Assert.AreEqual($"Provided type '{typeof(IInterfaceWithoutClientAttribute).Name}' must be annotated with '{typeof(ClientAttribute).Name}' attribute", ex.Message);
        }

        [Test, Order(1)]
        public void VerifyInitializationExceptionWhenNoConvertersProvided()
        {
            var ex = Assert.Throws<InitializationException>(() => RestClient.Instance.GetService<ITestInterface>().Get("1").Invoke());
            Assert.AreEqual($"There is no registered convertors found for the '{typeof(RestClient).Name}'. Please add at least one of it.", ex.Message);
        }

        [Test, Order(2)]
        public void VerifyInitializationExceptionWhenResponseConvertorIsNotRegistered()
        {
            var ex = Assert.Throws<InitializationException>(() => RestClient.Instance.GetService<ITestInterfaceValid>().GetAll().Invoke());
            Assert.AreEqual($"Response converter '{typeof(FakeResponseConverter).Name}' is not registered in the {typeof(RestClient).Name}", ex.Message);
        }

        [Test, Order(3)]
        public void VerifyGetAllRequestWhenNoLoggerForRestClient()
        {
            var expectedResponse = new Response
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseUri = new Uri("http://localhost:8080/api/{path_interceptor_param}/path/all"),
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("interceptor-header", "interceptor-header-value")
                },
                Cookies = new List<KeyValuePair<string, string>>(),
                ContentType = "application/json",
                OriginalResponse = null,
                ResponseBodyString = null,
                ElapsedTime = 1000
            };

            RestClient.Instance.AddResponseConvertor(new FakeResponseConverter());
            var actualResponse = RestClient.Instance.GetService<ITestInterfaceValid>().GetAll().Invoke();

            VerifyResponses(expectedResponse, actualResponse);
        }

        [Test, Order(3)]
        public void VerifyGetWithTestResponseWhenResultObjectIsDescendantOfResponse()
        {
            var response = new Response
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseUri = new Uri("http://localhost:8080/api/{path_interceptor_param}"),
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("interceptor-header", "interceptor-header-value")
                },
                Cookies = new List<KeyValuePair<string, string>>(),
                ContentType = "application/json",
                ElapsedTime = 1000
            };
            var expectedResponse = new TestResponse(response);

            RestClient.Instance.AddResponseConvertor(new FakeResponseConverter());
            var actualResponse = RestClient.Instance.GetService<ITestInterfaceValid>().GetWithTestResponse().Invoke();

            VerifyResponses(expectedResponse, actualResponse);
            Assert.IsInstanceOf<TestResponse>(actualResponse);
            Assert.AreEqual("application/json", actualResponse.TestString);
        }

        [Test, Order(4)]
        public void VerifyGetAllRequestWhenNoLoggingForRestClient()
        {
            var loggingDictionary = new Dictionary<string, string>();
            var restLogger = new RestLogger(s => loggingDictionary.Add(RestLoggerTests.BeforeConstKey, s),
                s => loggingDictionary.Add(RestLoggerTests.RequestConstKey, s),
                s => loggingDictionary.Add(RestLoggerTests.ResponseConstKey, s));
            RestClient.Instance.SetLogger(restLogger);
            var expectedResponse = new Response
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseUri = new Uri("http://localhost:8080/api/{path_interceptor_param}/path/all"),
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("interceptor-header", "interceptor-header-value")
                },
                Cookies = new List<KeyValuePair<string, string>>(),
                ContentType = "application/json",
                OriginalResponse = null,
                ResponseBodyString = null,
                ElapsedTime = 1000
            };

            RestClient.Instance.AddResponseConvertor(new FakeResponseConverter());
            var actualResponse = RestClient.Instance.GetService<ITestInterfaceValid>().GetAll().Invoke();

            VerifyResponses(expectedResponse, actualResponse);
            Assert.IsFalse(loggingDictionary.ContainsKey(RestLoggerTests.BeforeConstKey));
        }

        [Test, Order(5)]
        public void VerifyPatchRequestWhenLoggingForRestClientIsProvided()
        {
            var loggingDictionary = new Dictionary<string, string>();
            var restLogger = new RestLogger(s => loggingDictionary.Add(RestLoggerTests.BeforeConstKey, s),
                s => loggingDictionary.Add(RestLoggerTests.RequestConstKey, s),
                s => loggingDictionary.Add(RestLoggerTests.ResponseConstKey, s));
            RestClient.Instance.SetLogger(restLogger);
            var dynamicParameter = new DynamicParameter(AttributeType.Replaceable, "dynamicReplaceable", "2");

            var expectedResponse = new Response
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseUri = new Uri("http://localhost:8080/api/{path_interceptor_param}/path/1/2"),
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("interceptor-header", "interceptor-header-value")
                },
                Cookies = new List<KeyValuePair<string, string>>(),
                ContentType = "application/json",
                OriginalResponse = null,
                ResponseBodyString = null,
                ElapsedTime = 1000
            };

            RestClient.Instance.AddResponseConvertor(new FakeResponseConverter());
            var actualResponse = RestClient.Instance.GetService<ITestInterfaceValid>().Patch(1, dynamicParameter).Invoke();

            VerifyResponses(expectedResponse, actualResponse);
            Assert.IsTrue(loggingDictionary.ContainsKey(RestLoggerTests.BeforeConstKey));
            Assert.AreEqual("Send PATCH request to 'Nowhere' for unitTesting with the invalid argument index: invalid={5}", loggingDictionary[RestLoggerTests.BeforeConstKey]);
        }

        [Test, Order(6)]
        public void VerifyPostRequestWhenLoggingForRestClientIsProvided()
        {
            var loggingDictionary = new Dictionary<string, string>();
            var restLogger = new RestLogger(s => loggingDictionary.Add(RestLoggerTests.BeforeConstKey, s),
                s => loggingDictionary.Add(RestLoggerTests.RequestConstKey, s),
                s => loggingDictionary.Add(RestLoggerTests.ResponseConstKey, s));
            RestClient.Instance.SetLogger(restLogger);

            var body = new ResponseTests.UserJsonDto
            {
                Name = "Master",
                Age = 99
            };

            var response = new Response
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseUri = new Uri("http://localhost:8080/api/{path_interceptor_param}/path"),
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("x-api-key", "123"),
                    new KeyValuePair<string, string>("Authorization", "Bearer foo"),
                    new KeyValuePair<string, string>("interceptor-header", "interceptor-header-value")
                },
                Cookies = new List<KeyValuePair<string, string>>(),
                ContentType = "application/json",
                OriginalResponse = body,
                ResponseBodyString = body.ToString(),
                ElapsedTime = 1000
            };

            var expectedResponse = new ResponseGeneric<ResponseTests.UserJsonDto>(response);

            RestClient.Instance.AddResponseConvertor(new FakeResponseConverter());
            var actualResponse = RestClient.Instance.GetService<ITestInterfaceValid>().Post("Bearer foo", body).Invoke();

            VerifyResponses(expectedResponse, actualResponse);
            Assert.IsTrue(loggingDictionary.ContainsKey(RestLoggerTests.BeforeConstKey));
            Assert.AreEqual($"Send POST request to 'Nowhere' for unitTesting with the next parameters: Authorization=Bearer foo, Body={body}", loggingDictionary[RestLoggerTests.BeforeConstKey]);
        }

        private void VerifyResponses(Response expectedResponse, Response actualResponse)
        {
            Assert.AreEqual(expectedResponse.HttpStatusCode, actualResponse.HttpStatusCode);
            Assert.AreEqual(expectedResponse.ResponseUri, actualResponse.ResponseUri);
            Assert.AreEqual(expectedResponse.Headers, actualResponse.Headers);
            Assert.AreEqual(expectedResponse.Cookies, actualResponse.Cookies);
            Assert.AreEqual(expectedResponse.ContentType, actualResponse.ContentType);
            Assert.AreEqual(expectedResponse.OriginalResponse, actualResponse.OriginalResponse);
            Assert.AreEqual(expectedResponse.ResponseBodyString, actualResponse.ResponseBodyString);
            Assert.AreEqual(expectedResponse.ElapsedTime, actualResponse.ElapsedTime);
        }

    }

    public interface IInterfaceWithoutClientAttribute
    {
        [RequestMapping("GET", Path = "/path")]
        Func<Response> Get([QueryParameter("id")] string id);
    }

    public class FakeResponseConverter : IResponseConverter
    {
        public Response Convert(Func<Request> request)
        {
            var initialRequest = request.Invoke();
            return new Response
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseUri = new Uri(initialRequest.Url),
                Headers = initialRequest.Headers,
                Cookies = new List<KeyValuePair<string, string>>(),
                ContentType = "application/json",
                OriginalResponse = initialRequest.Body.Value,
                ResponseBodyString = initialRequest.Body.Value?.ToString(),
                ElapsedTime = 1000,
                Deserializer = initialRequest.Deserializer
            };
        }
    }
}
