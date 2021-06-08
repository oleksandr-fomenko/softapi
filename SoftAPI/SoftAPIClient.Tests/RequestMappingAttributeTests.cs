using NUnit.Framework;
using SoftAPIClient.Attributes;
using System.Net.Http;

namespace SoftAPIClient.Tests
{
    public class RequestMappingAttributeTests : AbstractTest
    {
        [Test]
        public void VerifyNullMethodValue()
        {
            var requestMappingAttribute = new RequestMappingAttribute(null);
            Assert.IsNull(requestMappingAttribute.Method);
            Assert.AreEqual(new string[0], requestMappingAttribute.Headers);
            Assert.IsNull(requestMappingAttribute.RequestInterceptor);
            Assert.IsNull(requestMappingAttribute.ResponseInterceptors);
            Assert.AreEqual(string.Empty, requestMappingAttribute.Path);
        }

        [Test]
        public void VerifyNotNullMethodValue()
        {
            var method = HttpMethod.Post;
            var requestMappingAttribute = new RequestMappingAttribute(method);
            Assert.AreEqual(method.ToString(), requestMappingAttribute.Method);
            Assert.AreEqual(new string[0], requestMappingAttribute.Headers);
            Assert.IsNull(requestMappingAttribute.RequestInterceptor);
            Assert.IsNull(requestMappingAttribute.ResponseInterceptors);
            Assert.AreEqual(string.Empty, requestMappingAttribute.Path);
        }

    }
}
