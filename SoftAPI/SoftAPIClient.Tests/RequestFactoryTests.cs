using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Exceptions;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using SoftAPIClient.Implementations;

namespace SoftAPIClient.Tests
{
    public class RequestFactoryTests : AbstractTest
    {
        private static readonly object PadLock = new object();
        private static TestDeserializer _testDeserializer = null;
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

        [Test]
        public void VerifyPostRequestWithAdditionalRequest()
        {
            var targetInterface = typeof(ITestInterfaceValidAdditionalRequest);
            const string methodName = "Post";

            var arguments = new object[] { null };

            var additionalInterceptor = new TestRequestAdditionalInterceptor();

            var expectedRequest = new Request
            {
                Url = "http://localhost:8080/api/{path_interceptor_param}/path/additional/{path_interceptor_param_additional}",
                Method = "POST",
                Deserializer = GetDeserializer(),
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("x-api-key", "123"),
                    new KeyValuePair<string, string>("interceptor-header", "interceptor-header-value"),
                    new KeyValuePair<string, string>("interceptor-header-additional", "interceptor-header-value-additional")
                },
                QueryParameters = new Dictionary<string, object>
                {
                    { "query_interceptor_param",  123},
                    { "query_interceptor_param_additional",  123}
                },
                PathParameters = new Dictionary<string, object>
                {
                    { "path_interceptor_param",  "v1"},
                    { "path_interceptor_param_additional",  "v1"}
                },
                FormDataParameters = new Dictionary<string, object>
                {
                    { "formData_interceptor_param",  "x"},
                    { "formData_interceptor_param_additional",  "x"}
                }
            };

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);
            var actualRequest = requestFactory.BuildRequest(additionalInterceptor);

            Assert.AreEqual(expectedRequest, actualRequest);
        }

        [Test]
        public void VerifyPostRequestWithAdditionalRequestWithUrlDynamicParameter()
        {
            var targetInterface = typeof(ITestInterfaceValidAdditionalRequest);
            const string methodName = "Post";

            var dynamicParameter = new DynamicParameter(AttributeType.Url, null, "http://localhost:8080");
            var arguments = new object[] { dynamicParameter };

            var expectedRequest = new Request
            {
                Url = "http://localhost:8080/api/{path_interceptor_param}/path",
                Method = "POST",
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("x-api-key", "123"),
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

        [Test]
        public void VerifyGetAllRequest()
        {
            var targetInterface = typeof(ITestInterfaceValid);
            const string methodName = "GetAll";

            var arguments = new object[] { };

            var expectedRequest = new Request
            {
                Deserializer = GetDeserializer(),
                Url = "http://localhost:8080/api/{path_interceptor_param}/path/all",
                Method = "GET",
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

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);
            var actualRequest = requestFactory.BuildRequest();

            Assert.AreEqual(expectedRequest, actualRequest);
        }

        [Test]
        public void VerifyPatchRequest()
        {
            var targetInterface = typeof(ITestInterfaceValid);
            const string methodName = "Patch";
            var dynamicParameter = new DynamicParameter(AttributeType.Replaceable, "dynamicReplaceable", "2");
            var arguments = new object[] { "1", dynamicParameter };

            var expectedRequest = new Request
            {
                Url = "http://localhost:8080/api/{path_interceptor_param}/path/1/2",
                
                Method = "PATCH",
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

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);
            var actualRequest = requestFactory.BuildRequest();

            Assert.AreEqual(expectedRequest, actualRequest);
            Assert.IsNotNull(requestFactory.ResponseInterceptors);
            Assert.IsNotEmpty(requestFactory.ResponseInterceptors);
        }

        [TestCase(AttributeType.Header)]
        [TestCase(AttributeType.Query)]
        [TestCase(AttributeType.FormData)]
        public void VerifyPatchRequestWhenDynamicAttributeValueIsNull(AttributeType attributeType)
        {
            var targetInterface = typeof(ITestInterfaceValid);
            const string methodName = "Patch";
            var dynamicParameter = new DynamicParameter(attributeType, "dynamicReplaceable", null);
            var arguments = new object[] { "1", dynamicParameter };

            var expectedRequest = new Request
            {
                Url = "http://localhost:8080/api/{path_interceptor_param}/path/1/{dynamicReplaceable}",

                Method = "PATCH",
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

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);
            var actualRequest = requestFactory.BuildRequest();

            Assert.AreEqual(expectedRequest, actualRequest);
        }

        [Test]
        public void VerifyPatchRequestWhenDynamicParameterIsUndefined()
        {
            var targetInterface = typeof(ITestInterfaceValid);
            const string methodName = "Patch";

            var dynamicParameter = new DynamicParameter(AttributeType.Undefined, "dynamicReplaceable", "2");
            var arguments = new object[] { "1", dynamicParameter };

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);
            var ex = Assert.Throws<InitializationException>(() => requestFactory.BuildRequest());
            Assert.AreEqual("The dynamic attribute-type should be initialized in the attribute or inside the DynamicParameter implementation", ex.Message);
        }

        [Test]
        public void VerifyGetInvalidHeaderFormatRequest()
        {
            var targetInterface = typeof(ITestInterfaceValid);
            const string methodName = "GetInvalidHeaderFormat";

            var arguments = new object[] { };

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);
            var ex = Assert.Throws<InitializationException>(() => requestFactory.BuildRequest());
            Assert.AreEqual("The following header 'x-api-key+123' is not specified of the format: key=value", ex.Message);
        }

        [Test]
        public void VerifyGetAllRequestWhenDynamicUrlAndDynamicSettingsAreProvided()
        {
            var targetInterface = typeof(ITestTestDynamicUrlInterface);
            const string methodName = "GetAll";
            var settings = new DynamicRequestSettings();
            var arguments = new object[] { settings };

            var expectedRequest = new Request
            {
                Url = "http://localhost:8080/path/all",
                Method = "GET",
                Settings = settings
            };

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);
            var actualRequest = requestFactory.BuildRequest();

            Assert.AreEqual(expectedRequest, actualRequest);
        }

        [Test]
        public void VerifyPatchRequestWhenDynamicParameterValueIsNull()
        {
            var targetInterface = typeof(ITestInterfaceValid);
            const string methodName = "Patch";
            var arguments = new object[] { "1", null };

            var expectedRequest = new Request
            {
                Url = "http://localhost:8080/api/{path_interceptor_param}/path/1/{dynamicReplaceable}",

                Method = "PATCH",
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

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);
            var actualRequest = requestFactory.BuildRequest();

            Assert.AreEqual(expectedRequest, actualRequest);
            Assert.IsNotNull(requestFactory.ResponseInterceptors);
            Assert.IsNotEmpty(requestFactory.ResponseInterceptors);
        }

        [Test]
        public void VerifyPatchDynamicReplaceableRequest()
        {
            var targetInterface = typeof(ITestInterfaceValid);
            const string methodName = "PatchDynamicReplaceable";
            var dynamicParameter = new DynamicParameter("dynamicReplaceable", "2");
            var arguments = new object[] { "1", dynamicParameter };

            var expectedRequest = new Request
            {
                Url = "http://localhost:8080/api/{path_interceptor_param}/path/1/2",

                Method = "PATCH",
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

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);
            var actualRequest = requestFactory.BuildRequest();

            Assert.AreEqual(expectedRequest, actualRequest);
            Assert.IsNotNull(requestFactory.ResponseInterceptors);
            Assert.IsNotEmpty(requestFactory.ResponseInterceptors);
        }

        public static TestDeserializer GetDeserializer()
        {
            if(_testDeserializer == null)
            {
                lock (PadLock)
                {
                    if (_testDeserializer == null)
                    {
                        _testDeserializer = new TestDeserializer();
                        return _testDeserializer;
                    }
                }
            }
            return _testDeserializer;
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

    [Client(typeof(FakeResponseConverter), Url = "http://localhost:8080", Path = "/api/{path_interceptor_param}", RequestInterceptor = typeof(TestRequestInterceptor), ResponseInterceptors = new []{typeof(TestResponseInterceptor) }) ]
    public interface ITestInterfaceValid
    {
        [RequestMapping("GET", Path = "/path/all", RequestInterceptor = typeof(TestRequestSpecificInterceptor))]
        Func<Response> GetAll();

        [RequestMapping("GET", Path = "/path/all")]
        Func<ResponseGeneric<ResponseTests.UserJsonDto>> GetAllGeneric();

        [RequestMapping("GET", Headers = new[] { "x-api-key+123" })]
        Func<Response> GetInvalidHeaderFormat();

        [Log("Send PATCH request to 'Nowhere' for unitTesting with the invalid argument index: invalid={5}")]
        [RequestMapping("PATCH", Path = "/path/{pathId}/{dynamicReplaceable}")]
        Func<Response> Patch([ReplaceableParameter("pathId")] int pathId, [DynamicParameter] IDynamicParameter dynamicParameter);

        [Log("Send PATCH request to 'Nowhere' for unitTesting with the invalid argument index: invalid={5}")]
        [RequestMapping("PATCH", Path = "/path/{pathId}/{dynamicReplaceable}")]
        Func<Response> PatchDynamicReplaceable([ReplaceableParameter("pathId")] int pathId, [DynamicParameter(AttributeType.Replaceable)] IDynamicParameter dynamicParameter);

        [Log("Send POST request to 'Nowhere' for unitTesting with the next parameters: Authorization={0}, Body={1}")]
        [RequestMapping("POST", Path = "/path", Headers = new[] { "x-api-key=123" }, ResponseInterceptors = new[] { typeof(TestResponseInterceptor) })]
        Func<ResponseGeneric<ResponseTests.UserJsonDto>> Post([HeaderParameter("Authorization")] string authorization, [Body] ResponseTests.UserJsonDto body);

        [RequestMapping("GET")]
        Func<TestResponse> GetWithTestResponse();
    }

    [Client(DynamicUrlKey = "http://localhost:8080", DynamicUrlType = typeof(TestDynamicUrl), Logger = typeof(CustomRestLogger))]
    public interface ITestTestDynamicUrlInterface
    {
        [RequestMapping("GET", Path = "/path/all")]
        Func<Response> GetAll([Settings] DynamicRequestSettings requestSettings);
    }

    [Client(typeof(FakeResponseConverter), Path = "/api/{path_interceptor_param}", RequestInterceptor = typeof(TestRequestInterceptor))]
    public interface ITestInterfaceValidAdditionalRequest
    {
        [Log("Send POST request to 'Nowhere' for unitTesting with the next parameters: Authorization={0}, Body={1}")]
        [RequestMapping("POST", Path = "/path", Headers = new[] { "x-api-key=123" })]
        Func<ResponseGeneric<ResponseTests.UserJsonDto>> Post([DynamicParameter] IDynamicParameter dynamic);
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

    public class TestRequestSpecificInterceptor : IInterceptor
    {
        public Request Intercept()
        {
            return new Request
            {
                Deserializer = RequestFactoryTests.GetDeserializer(),
            };
        }
    }

    public class TestRequestAdditionalInterceptor : IInterceptor
    {
        public Request Intercept()
        {
            return new Request
            {
                Url = "http://localhost:8080",
                Deserializer = RequestFactoryTests.GetDeserializer(),
                Path = "/additional/{path_interceptor_param_additional}",
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("interceptor-header-additional", "interceptor-header-value-additional")
                },
                QueryParameters = new Dictionary<string, object>
                {
                    { "query_interceptor_param_additional",  123}
                },
                PathParameters = new Dictionary<string, object>
                {
                    { "path_interceptor_param_additional",  "v1"}
                },
                FormDataParameters = new Dictionary<string, object>
                {
                    { "formData_interceptor_param_additional",  "x"}
                }
            };
        }
    }

    public class TestDeserializer : IResponseDeserializer
    {
        public T Convert<T>(string response)
        {
            return default;
        }
    }

    public class TestResponseInterceptor : IResponseInterceptor
    {
        public void ProcessResponse(Response response)
        {
            //do nothing
        }
    }

    public class TestDynamicUrl : IDynamicUrl
    {
        public string GetUrl(string key)
        {
            return key;
        }
    }

    public class CustomRestLogger : RestLogger
    {
        public CustomRestLogger() : base(s => DoNothing(), s => DoNothing(), s => DoNothing())
        {
        }

        private static void DoNothing()
        {
        }
    }

    public class TestResponse : Response
    {
        public TestResponse(Response response)
        {
            HttpStatusCode = response.HttpStatusCode;
            ResponseUri = response.ResponseUri;
            Headers = response.Headers;
            Cookies = response.Cookies;
            ContentType = response.ContentType;
            OriginalRequest = response.OriginalRequest;
            OriginalResponse = response.OriginalResponse;
            ResponseBodyString = response.ResponseBodyString;
            ElapsedTime = response.ElapsedTime;
            Deserializer = response.Deserializer;
        }
        public string TestString => ContentType;
    }
}
