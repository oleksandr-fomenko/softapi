using NUnit.Framework;
using SoftAPIClient.Core.Auth;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Tests
{
    public class AuthBasic64Tests : AbstractTest
    {
        [Test]
        public void VerifyAttributeType()
        {
            var authBasic64 = new AuthBasic64("Ivan", "WeakPassword");
            Assert.AreEqual(AttributeType.Header, authBasic64.GetAttributeType());
        }

        [Test]
        public void VerifyAttributeValue()
        {
            var authBasic64 = new AuthBasic64("Ivan", "WeakPassword");
            var pair = authBasic64.GetValue();
            var expectedKey = pair.Key; 
            var expectedValue = pair.Value;
            Assert.AreEqual("Authorization", expectedKey);
            Assert.AreEqual("Basic SXZhbjpXZWFrUGFzc3dvcmQ=", expectedValue);
        }

    }
}
