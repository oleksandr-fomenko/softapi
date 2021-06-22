using System.Globalization;
using NUnit.Framework;

namespace SoftAPIClient.RestSharpNewtonsoft.Tests
{
    public class RestSharpCustomDateTimeConverterTests : AbstractTest
    {
        [Test]
        public void VerifyThatCustomDateTimeConverterHasDefaultParameters()
        {
            var restSharpCustomDateTimeConverter = new RestSharpCustomDateTimeConverter();

            Assert.AreEqual("yyyy-MM-ddTHH:mm:ss", restSharpCustomDateTimeConverter.DateTimeFormat);
            Assert.AreEqual(DateTimeStyles.AssumeUniversal, restSharpCustomDateTimeConverter.DateTimeStyles);
        }

    }
}
