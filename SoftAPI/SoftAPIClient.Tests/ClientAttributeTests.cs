using NUnit.Framework;
using SoftAPIClient.Attributes;
using System;

namespace SoftAPIClient.Tests
{
    public class ClientAttributeTests : AbstractTest
    {
        [Test]
        public void VerifyDefaultValue()
        {
            var clientAttribute = new ClientAttribute();
            Assert.IsNull(clientAttribute.ResponseConverterType);
            Assert.IsNull(clientAttribute.Url);
            Assert.AreEqual(string.Empty, clientAttribute.DynamicUrlKey);
            Assert.IsNull(clientAttribute.Logger);
            Assert.IsNull(clientAttribute.DynamicUrlType);
            Assert.IsNull(clientAttribute.RequestInterceptor);
            Assert.IsNull(clientAttribute.ResponseInterceptors);
            Assert.AreEqual(string.Empty, clientAttribute.Path);
        }

        [Test]
        public void VerifyValueWithTypeValue()
        {
            var type = typeof(Type);
            var clientAttribute = new ClientAttribute(type);
            Assert.AreEqual(type, clientAttribute.ResponseConverterType);
            Assert.IsNull(clientAttribute.Url);
            Assert.AreEqual(string.Empty, clientAttribute.DynamicUrlKey);
            Assert.IsNull(clientAttribute.Logger);
            Assert.IsNull(clientAttribute.DynamicUrlType);
            Assert.IsNull(clientAttribute.RequestInterceptor);
            Assert.IsNull(clientAttribute.ResponseInterceptors);
            Assert.AreEqual(string.Empty, clientAttribute.Path);
        }

    }
}
