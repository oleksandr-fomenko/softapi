using NUnit.Framework;
using SoftAPIClient.Core;

namespace SoftAPIClient.Tests
{
    public class FileParameterTests : AbstractTest
    {
        [Test]
        public void VerifyFileParameter()
        {
            const string name = "testFile";
            const string fileName = "test.jpeg";
            const string contentType = "image/jpeg";
            var bytes = new byte[]{1, 2, 3};
            var fileParameter = new FileParameter(name, bytes, fileName, contentType);

            Assert.AreEqual(name, fileParameter.Name);
            Assert.AreEqual(fileName, fileParameter.FileName);
            Assert.AreEqual(contentType, fileParameter.ContentType);
            Assert.AreEqual(bytes, fileParameter.Bytes);
            Assert.AreEqual($"Name={name}, FileName={fileName}, ContentType={contentType}", 
                fileParameter.ToString());
        }
    }
}
