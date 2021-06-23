using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Exceptions;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftAPIClient.Tests
{
    public class RequestFactoryTests : AbstractTest
    {
        [Test]
        public void VerifyInitializationExceptionWhenNullUrlProvided()
        {
            var targetInterface = typeof(ITestInterface);
            const string methodName = "Get";
            var arguments = new []{ "1" };

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);

            var ex = Assert.Throws<InitializationException>(() => requestFactory.BuildRequest());
            Assert.AreEqual($"The result URL is not defined at the interface '{nameof(ITestInterface)}' and method '{methodName}'", ex.Message);
        }

        [Test]
        public void VerifyInitializationExceptionWhenDifferentArgumentsProvided()
        {
            var targetInterface = typeof(ITestInterface);
            const string methodName = "Get";
            var arguments = new[] { "1" , "2"};

            var ex = Assert.Throws<InitializationException>(() => new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments));
            Assert.AreEqual($"Argument count '{arguments.Length}' and MethodInfo count '{1}' " +
                    $"is not matched for the method '{methodName}' in type '{nameof(ITestInterface)}'", ex.Message);
        }

        [Test]
        public void VerifyDefaultRequestFactoryProperties()
        {
            var targetInterface = typeof(ITestInterface);
            const string methodName = "Get";
            var arguments = new[] { "1" };

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);

            Assert.AreEqual(Enumerable.Empty<IResponseInterceptor>(), requestFactory.ResponseInterceptors);
            Assert.IsNull(requestFactory.ResponseConverterType);
            Assert.IsNull(requestFactory.Logger);
        }

        [Test]
        public void VerifyPostRequest()
        {
            var targetInterface = typeof(ITestInterfaceValid);
            const string methodName = "Post";
            var body = new ResponseTests.UserJsonDto
            {
                Name = "Master",
                Age = 99
            };
            var arguments = new object[] { "Bearer foo", body };

            var expectedRequest = new Request
            {
                Url = "http://localhost:8080/api/{path_interceptor_param}/path",
                Body = new KeyValuePair<BodyType, object>(BodyType.Json, body),
                Method = "POST",
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("x-api-key", "123"),
                    new KeyValuePair<string, string>("Authorization", "Bearer foo"),
                    new KeyValuePair<string, string>("interceptor-header", "interceptor-header-value")
                },
                QueryParameters = new Dictionary<string, object>
                {
                    { "query_interceptor_param",  123}
                },
                PathParameters = new Dictionary<string, object>
                {
                    { "path_interceptor_param",  "v1"}
                },
                FormDataParameters = new Dictionary<string, object>
                {
                    { "formData_interceptor_param",  "x"}
                }
            };

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);
            var actualRequest = requestFactory.BuildRequest();

            Assert.AreEqual(expectedRequest, actualRequest);
        }

    }

    [Client]
    public interface ITestInterface
    {
        [RequestMapping("GET", Path = "/path/all")]
        Func<Response> GetAll();

        [RequestMapping("GET", Path = "/path")]
        Func<Response> Get([QueryParameter("id")] string id);

        [RequestMapping("PATCH", Path = "/path/{pathId}")]
        Func<Response> Patch([PathParameter("pathId")] int pathId, [HeaderParameter("Authorization")] string authorization, [DynamicParameter] IDynamicParameter dynamicParameter);

        [RequestMapping("POST", Path = "/path", Headers = new []{"x-api-key=123"})]
        Func<Response> Post([HeaderParameter("Authorization")] string authorization, [Body] ResponseTests.UserJsonDto body);
    }

    [Client(Url = "http://localhost:8080", Path = "/api/{path_interceptor_param}", RequestInterceptor = typeof(TestRequestInterceptor))]
    public interface ITestInterfaceValid
    {
        [RequestMapping("GET", Path = "/path/all")]
        Func<Response> GetAll();

        [RequestMapping("GET", Path = "/path")]
        Func<Response> Get([QueryParameter("id")] string id);

        [RequestMapping("PATCH", Path = "/path/{pathId}")]
        Func<Response> Patch([PathParameter("pathId")] int pathId, [HeaderParameter("Authorization")] string authorization, [DynamicParameter] IDynamicParameter dynamicParameter);

        [RequestMapping("POST", Path = "/path", Headers = new[] { "x-api-key=123" })]
        Func<Response> Post([HeaderParameter("Authorization")] string authorization, [Body] ResponseTests.UserJsonDto body);
    }

    public class TestRequestInterceptor : IInterceptor
    {
        public Request Intercept()
        {
            return new Request
            {
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("interceptor-header", "interceptor-header-value")
                },
                QueryParameters = new Dictionary<string, object>
                {
                    { "query_interceptor_param",  123}
                },
                PathParameters = new Dictionary<string, object>
                {
                    { "path_interceptor_param",  "v1"}
                },
                FormDataParameters = new Dictionary<string, object>
                {
                    { "formData_interceptor_param",  "x"}
                }
            };
        }
    }
}
