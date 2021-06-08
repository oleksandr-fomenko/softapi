using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Tests
{
    public class DynamicParameterAttributeTests : AbstractTest
    {
        [Test]
        public void VerifyDefaultDynamicParameterAttributeType()
        {
            var dynamicParameterAttribute = new DynamicParameterAttribute();
            Assert.AreEqual(AttributeType.Undefined, dynamicParameterAttribute.AttributeType);
            Assert.IsNull(dynamicParameterAttribute.Value);
        }

        [Test]
        public void VerifyDynamicParameterAttributeType()
        {
            var dynamicParameterAttribute = new DynamicParameterAttribute(AttributeType.Header);
            Assert.AreEqual(AttributeType.Header, dynamicParameterAttribute.AttributeType);
            Assert.IsNull(dynamicParameterAttribute.Value);
        }
    }
}
