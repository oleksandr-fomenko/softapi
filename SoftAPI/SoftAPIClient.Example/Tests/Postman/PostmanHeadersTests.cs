using System.Linq;
using System.Net;
using NUnit.Framework;
using SoftAPIClient.Core;
using SoftAPIClient.Example.Core;
using SoftAPIClient.Example.Models.Postman;
using SoftAPIClient.Example.Services.Postman;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Tests.Postman
{
    public class PostmanHeadersTests : AbstractTest
    {
        private const string HeaderName = "some-header-name";
        private const string HeaderValue = "some-header-value";

        [Test]
        public void VerifyDynamicParameterByInterface()
        {
            var response = GetService<IPostmanEchoHeadersService>()
                .DynamicParameterByInterface(new DynamicParameter(AttributeType.Header, HeaderName, HeaderValue))
                .Invoke();

            VerifyHeaderResponseData(response);

            response = GetService<IPostmanEchoHeadersService>()
                .DynamicParameterByInterface(new CustomDynamicHeader(HeaderName, HeaderValue))
                .Invoke();

            VerifyHeaderResponseDataDynamic(response);
        }

        [Test]
        public void VerifyDynamicParameter()
        {
            var response = GetService<IPostmanEchoHeadersService>()
                .DynamicParameter(new DynamicParameter(AttributeType.Header, HeaderName, HeaderValue))
                .Invoke();

            VerifyHeaderResponseData(response);

            response = GetService<IPostmanEchoHeadersService>()
                .DynamicParameter(new CustomDynamicHeader(HeaderName, HeaderValue))
                .Invoke();

            VerifyHeaderResponseDataDynamic(response);
        }

        [Test]
        public void VerifyDynamicParameterWithCustomHeader()
        {
            var response = GetService<IPostmanEchoHeadersService>()
                .DynamicParameterByDefinedAttributeType(new DynamicParameter(HeaderName, HeaderValue))
                .Invoke();

            VerifyHeaderResponseData(response);

            response = GetService<IPostmanEchoHeadersService>()
                .DynamicParameterByDefinedAttributeType(new CustomDynamicHeader(HeaderName, HeaderValue))
                .Invoke();

            VerifyHeaderResponseDataDynamic(response);
        }

        [Test]
        public void VerifyDynamicParameterByCustomParameter()
        {
            var response = GetService<IPostmanEchoHeadersService>()
                .DynamicParameterByCustomParameter(new CustomDynamicHeader(HeaderName, HeaderValue))
                .Invoke();

            VerifyHeaderResponseDataDynamic(response);
        }

        private void VerifyHeaderResponseData(ResponseGeneric<PostmanResponse> response)
        {
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            var actualHeaders = response.Body.Headers;
            Assert.NotNull(actualHeaders);
            Assert.True(actualHeaders.ContainsKey(HeaderName));
            Assert.AreEqual(HeaderValue, actualHeaders[HeaderName]);
        }

        private void VerifyHeaderResponseDataDynamic(ResponseGeneric<PostmanResponse> response)
        {
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            var actualHeaders = response.Body.Headers;
            Assert.NotNull(actualHeaders);
            Assert.True(actualHeaders.Any(h => h.Key.StartsWith(HeaderName)));
            Assert.True(actualHeaders.Any(h => h.Value.StartsWith(HeaderValue)));
        }
    }
}
