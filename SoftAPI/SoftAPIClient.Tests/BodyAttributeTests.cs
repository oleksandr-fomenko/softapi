using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Tests
{
    public class BodyAttributeTests : AbstractTest
    {
        [Test]
        public void VerifyDefaultBodyType()
        {
            var bodyAttribute = new BodyAttribute();
            Assert.AreEqual(BodyType.Json, bodyAttribute.BodyType);
            Assert.IsNull(bodyAttribute.Value);
        }

        [Test]
        public void VerifyBodyType()
        {
            var bodyAttribute = new BodyAttribute(BodyType.Xml);
            Assert.AreEqual(BodyType.Xml, bodyAttribute.BodyType);
            Assert.IsNull(bodyAttribute.Value);
        }
    }
}
