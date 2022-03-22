using System;
using System.Collections.Generic;
using NUnit.Framework;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.RestSharpNewtonsoft.Tests
{
    public class RestSharpSerializerTests : AbstractTest
    {
        [TestCaseSource(nameof(GetTestData))]
        public void VerifyValidSerializer(Tuple<IObjectSerializer, object, string> inputData)
        {
            var (serializer, obj, expectedResult) = inputData;
            var actualResult = serializer.Convert(obj);
            Assert.AreEqual(expectedResult, actualResult);
        }

        private static IEnumerable<Tuple<IObjectSerializer, object, string>> GetTestData()
        {
            var jsonObj = new ResponseTests.UserJsonDto
            {
                Name = "Ivan",
                Age = 12
            };
            yield return new Tuple<IObjectSerializer, object, string>(new RestSharpJsonSerializer(), jsonObj, "{\"Name\":\"" + jsonObj.Name + "\",\"Age\":" + jsonObj.Age + "}");
            var xmlObj = new UserXmlDto
            {
                Name = "Alex",
                Age = 15
            };
            yield return new Tuple<IObjectSerializer, object, string>(new RestSharpXmlSerializer(), xmlObj, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<root>\r\n  <age>" + xmlObj.Age + "</age>\r\n  <name>" + xmlObj.Name + "</name>\r\n</root>");
        }
    }
}
