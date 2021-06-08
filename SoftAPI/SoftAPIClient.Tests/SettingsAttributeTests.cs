using NUnit.Framework;
using SoftAPIClient.Attributes;

namespace SoftAPIClient.Tests
{
    public class SettingsAttributeTests : AbstractTest
    {
        [Test]
        public void VerifyValueIsNull()
        {
            var bodyAttribute = new SettingsAttribute();
            Assert.IsNull(bodyAttribute.Value);
        }
    }
}
