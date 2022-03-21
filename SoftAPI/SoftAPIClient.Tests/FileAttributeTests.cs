using NUnit.Framework;
using SoftAPIClient.Attributes;

namespace SoftAPIClient.Tests
{
    public class FileAttributeTests : AbstractTest
    {
        [Test]
        public void VerifyFileAttribute()
        {
            var fileAttribute = new FileAttribute();
            Assert.IsNull(fileAttribute.Value);
        }
    }
}
