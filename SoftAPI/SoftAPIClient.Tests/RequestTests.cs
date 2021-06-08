using NUnit.Framework;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;
using System.Collections.Generic;
using System.Linq;
using static SoftAPIClient.Tests.ResponseTests;

namespace SoftAPIClient.Tests
{
    public class RequestTests : AbstractTest
    {

        [Test]
        public void VerifyRequest()
        {
            var url = "http://localhost:8080";
            var method = "POST";
            var path = "/path";
            var pathParameters = new Dictionary<string, object> { { "path_int_key", 1}, { "path_string_key", "string" } };
            var queryParameters = new Dictionary<string, object> { { "query_int_key", 1}, { "query_string_key", "string" } };
            var formDataParameters = new Dictionary<string, object> { { "formData_int_key", 1}, { "formData_string_key", "string" } };
            var headers = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Content-Type", "application/json") };
            var body = new KeyValuePair<BodyType, object>(BodyType.Json, new UserDto { Age = 15, Name = "Ivan"});
            IResponseDeserializer deserializer = null;
            var dynamicRequestSettings = new DynamicRequestSettings();

            var request = new Request
            {
                Url = url,
                Method = method,
                Path = path,
                PathParameters = pathParameters,
                QueryParameters = queryParameters,
                FormDataParameters = formDataParameters,
                Headers = headers,
                Body = body,
                Deserializer = deserializer,
                Settings = dynamicRequestSettings
            };

            Assert.AreEqual(url, request.Url);
            Assert.AreEqual(method, request.Method);
            Assert.AreEqual(path, request.Path);
            Assert.AreEqual(pathParameters, request.PathParameters);
            Assert.AreEqual(queryParameters, request.QueryParameters);
            Assert.AreEqual(formDataParameters, request.FormDataParameters);
            Assert.AreEqual(headers, request.Headers);
            Assert.AreEqual(body, request.Body);
            Assert.AreEqual(deserializer, request.Deserializer);
            Assert.AreEqual(dynamicRequestSettings, request.Settings);
        }

        [Test]
        public void VerifyDefaultRequest()
        {
            var request = new Request();

            Assert.IsNull(request.Url);
            Assert.IsNull(request.Method);
            Assert.AreEqual(string.Empty, request.Path);
            Assert.AreEqual(Enumerable.Empty<IDictionary<string, object>>(), request.PathParameters);
            Assert.AreEqual(Enumerable.Empty<IDictionary<string, object>>(), request.QueryParameters);
            Assert.AreEqual(Enumerable.Empty<IDictionary<string, object>>(), request.FormDataParameters);
            Assert.AreEqual(Enumerable.Empty<IList<KeyValuePair<string, string>>>(), request.Headers);
            Assert.AreEqual(default(KeyValuePair<BodyType, object>), request.Body);
            Assert.IsNull(request.Deserializer);
            Assert.IsNull(request.Settings);
        }

    
    }
}
