using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using SoftAPIClient.Core;
using SoftAPIClient.Implementations;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Tests
{
    public class RestLoggerTests : AbstractTest
    {
        public const string BeforeConstKey = "BEFORE";
        public const string RequestConstKey = "REQUEST";
        public const string ResponseConstKey = "RESPONSE";
        private const string BeforeConstValue = "Sending request...";

        [TestCaseSource(nameof(GetRequestTestData))]
        public void VerifyGetRequestMessage(KeyValuePair<Request, string> input)
        {
            var dictionaryResult = new Dictionary<string, string>();
            var restLogger = new RestLogger(s => dictionaryResult.Add(BeforeConstKey, s),
                s => dictionaryResult.Add(RequestConstKey, s),
                s => dictionaryResult.Add(ResponseConstKey, s));

            var expectedMessage = GetTestDataFileContent(input.Value);
            restLogger.LogRequest(input.Key);

            Assert.AreEqual(1, dictionaryResult.Count);
            Assert.True(dictionaryResult.ContainsKey(RequestConstKey));
            Assert.AreEqual(expectedMessage, dictionaryResult[RequestConstKey]);
        }

        [TestCaseSource(nameof(GetResponseTestData))]
        public void VerifyGetResponseMessage(KeyValuePair<Response, string> input)
        {
            var dictionaryResult = new Dictionary<string, string>();
            var restLogger = new RestLogger(s => dictionaryResult.Add(BeforeConstKey, s),
                s => dictionaryResult.Add(RequestConstKey, s),
                s => dictionaryResult.Add(ResponseConstKey, s));

            var expectedMessage = GetTestDataFileContent(input.Value);
            restLogger.LogResponse(input.Key);

            Assert.AreEqual(1, dictionaryResult.Count);
            Assert.True(dictionaryResult.ContainsKey(ResponseConstKey));
            Assert.AreEqual(expectedMessage, dictionaryResult[ResponseConstKey]);
        }

        [Test]
        public void VerifyLogBeforeMethod()
        {
            var dictionaryResult = new Dictionary<string, string>();
            var restLogger = new RestLogger(s => dictionaryResult.Add(BeforeConstKey, s), 
                s => dictionaryResult.Add(RequestConstKey, s), 
                s => dictionaryResult.Add(ResponseConstKey, s));

            restLogger.LogBefore(null);

            Assert.AreEqual(0, dictionaryResult.Count);

            restLogger.LogBefore(BeforeConstValue);
            Assert.AreEqual(1, dictionaryResult.Count);
            Assert.True(dictionaryResult.ContainsKey(BeforeConstKey));
            Assert.AreEqual(BeforeConstValue, dictionaryResult[BeforeConstKey]);
        }

        private static IEnumerable<KeyValuePair<Request, string>> GetRequestTestData()
        {
            yield return new KeyValuePair<Request, string>(new Request
            {
                Url = "http://localhost:8080",
                Method = "POST",
                Path = "/path",
                PathParameters = new Dictionary<string, object> { { "path_int_key", 1 }, { "path_string_key", "string" } },
                QueryParameters = new Dictionary<string, object> { { "query_int_key", 1 }, { "query_string_key", "string" } },
                FormDataParameters = new Dictionary<string, object> { { "formData_int_key", 1 }, { "formData_string_key", "string" } },
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Content-Type", "application/json"),
                    new KeyValuePair<string, string>("Authorization", "Bearer foo")
                },
                Body = new KeyValuePair<BodyType, object>(BodyType.Json, new ResponseTests.UserJsonDto { Age = 15, Name = "Ivan" }),
                Deserializer = null,
                Settings = new DynamicRequestSettings()
            }, "ExpectedRequestMessage_1.txt");
            yield return new KeyValuePair<Request, string>(new Request
            {
                Url = "http://localhost:8080",
                Method = "POST",
                Path = "/path"
            }, "ExpectedRequestMessage_2.txt");
        }

        private static IEnumerable<KeyValuePair<Response, string>> GetResponseTestData()
        {
            yield return new KeyValuePair<Response, string>(new Response
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseUri = new Uri("http://localhost:8080"),
                Headers = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Content-Type", "application/json"),
                    new KeyValuePair<string, string>("Authorization", "Bearer foo")
                },
                Cookies = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Cookie1", "CookiesValue") },
                ContentType = "application/json",
                ResponseBodyString = "{\"name\":\"Ivan\",\"age\":18}",
                ElapsedTime = 1000
            }, "ExpectedResponseMessage_1.txt");
            yield return new KeyValuePair<Response, string>(new Response
            {
                HttpStatusCode = HttpStatusCode.BadRequest,
                ResponseUri = new Uri("http://localhost:8080"),
                ContentType = "text/plain",
                ElapsedTime = 50
            }, "ExpectedResponseMessage_2.txt");
        }
    }
}
