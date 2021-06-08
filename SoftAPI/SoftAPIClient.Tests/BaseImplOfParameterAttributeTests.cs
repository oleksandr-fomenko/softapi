using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.Attributes.Base;
using System.Collections.Generic;

namespace SoftAPIClient.Tests
{
    public class BaseImplOfParameterAttributeTests : AbstractTest
    {

        [TestCaseSource(nameof(GetTestData))]
        public void VerifyValue(KeyValuePair<string, BaseParameterAttribute> testData)
        {
            var expectedValue = testData.Key;
            var parameter = testData.Value;

            Assert.AreEqual(expectedValue, parameter.Value);
        }

        private static IEnumerable<KeyValuePair<string, BaseParameterAttribute>> GetTestData()
        {
            yield return new KeyValuePair<string, BaseParameterAttribute>("testPathParameter", new PathParameterAttribute("testPathParameter"));
            yield return new KeyValuePair<string, BaseParameterAttribute>("testReplaceableParameter", new ReplaceableParameterAttribute("testReplaceableParameter"));
            yield return new KeyValuePair<string, BaseParameterAttribute>("testQueryParameter", new QueryParameterAttribute("testQueryParameter"));
            yield return new KeyValuePair<string, BaseParameterAttribute>("testFormDataParameter", new FormDataParameterAttribute("testFormDataParameter"));
            yield return new KeyValuePair<string, BaseParameterAttribute>("testHeaderParameter", new HeaderParameterAttribute("testHeaderParameter"));
            yield return new KeyValuePair<string, BaseParameterAttribute>("testLog", new LogAttribute("testLog"));
        }
    }
}
