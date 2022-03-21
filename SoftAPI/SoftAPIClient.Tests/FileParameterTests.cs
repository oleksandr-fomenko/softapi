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
            var fileParameter2 = new FileParameter(name, bytes, fileName, contentType);

            Assert.IsTrue(fileParameter.Equals(fileParameter2));
            Assert.AreEqual(name, fileParameter.Name);
            Assert.AreEqual(fileName, fileParameter.FileName);
            Assert.AreEqual(contentType, fileParameter.ContentType);
            Assert.AreEqual(bytes, fileParameter.Bytes);
            Assert.AreEqual($"Name={name}, FileName={fileName}, ContentType={contentType}", 
                fileParameter.ToString());
        }

        [Test]
        public void VerifyFileParameterNotEqual()
        {
            const string name = "testFile";
            const string fileName = "test.jpeg";
            const string contentType = "image/jpeg";
            var bytes = new byte[] { 1, 2, 3 };
            var fileParameter = new FileParameter(name, bytes, fileName, contentType);
            var fileParameter2 = new FileParameter(name + 1, new byte[] { 1, 2, 3, 4 }, fileName + 1, contentType + 1);

            Assert.IsFalse(fileParameter.Equals(fileParameter2));
        }

        [Test]
        public void VerifyEqualsCasesAndNullableHashCode()
        {
            var fileParameter = new FileParameter(null, null, null, null);
            var fileParameter2 = fileParameter;
            Assert.IsFalse(fileParameter.Equals(null));
            Assert.IsTrue(fileParameter.Equals(fileParameter2));
            Assert.IsFalse(fileParameter.Equals(new object()));
            Assert.IsTrue(fileParameter.GetHashCode(fileParameter) == 0);
            Assert.IsTrue(fileParameter.GetHashCode() == 0);
        }
    }
}
