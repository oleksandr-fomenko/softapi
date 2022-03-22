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
            Assert.IsNull(bodyAttribute.Name);
        }

        [Test]
        public void VerifyBodyType()
        {
            const string name = "test";
            var bodyAttribute = new BodyAttribute(BodyType.Xml, name);
            Assert.AreEqual(BodyType.Xml, bodyAttribute.BodyType);
            Assert.AreEqual(name, bodyAttribute.Name);
        }
    }
}
