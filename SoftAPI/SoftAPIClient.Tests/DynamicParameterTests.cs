using NUnit.Framework;
using SoftAPIClient.Core;
using SoftAPIClient.MetaData;
using System;
using System.Collections.Generic;

namespace SoftAPIClient.Tests
{
    public class DynamicParameterTests : AbstractTest
    {
        [TestCaseSource(nameof(GetTestData))]
        public void VerifyDynamicParameter(Tuple<DynamicParameter, string, string, AttributeType> testData)
        {
            var dynamicParameter = testData.Item1;
            var name = testData.Item2;
            var value = testData.Item3;
            var attributeType = testData.Item4;
            Assert.AreEqual(attributeType, dynamicParameter.GetAttributeType());
            Assert.AreEqual(new KeyValuePair<string, string>(name, value), dynamicParameter.GetValue());
            Assert.AreEqual($"AttributeType={attributeType}, {name}={value}", dynamicParameter.ToString());
        }

        private static IEnumerable<Tuple<DynamicParameter, string, string, AttributeType>> GetTestData()
        {
            yield return new Tuple<DynamicParameter, string, string, AttributeType>(new DynamicParameter(), null, null, AttributeType.Undefined);
            yield return new Tuple<DynamicParameter, string, string, AttributeType>(new DynamicParameter("someName", "someValue"), "someName", "someValue", AttributeType.Undefined);
            yield return new Tuple<DynamicParameter, string, string, AttributeType>(new DynamicParameter(AttributeType.Url, "someName", "someValue"), "someName", "someValue", AttributeType.Url);
        }
    }
}
