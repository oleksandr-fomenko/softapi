using System;
using System.Collections.Generic;
using NUnit.Framework;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.Implementations;

namespace SoftAPIClient.Tests
{
    public class SerializerTests : AbstractTest
    {
        [TestCaseSource(nameof(GetTestData))]
        public void WhenValidObjectIsProvidedStringIsCreated(Tuple<IObjectSerializer, object, string> testData)
        {
            var (serializer, inputObj, expectedString) = testData;
            var actualString = serializer.Convert(inputObj);

            Assert.AreEqual(expectedString, actualString);
        }


        private static IEnumerable<Tuple<IObjectSerializer, object, string>> GetTestData()
        {
            const int age = 15; 
            const string name = "Alex"; 
            yield return new Tuple<IObjectSerializer, object, string>(new DataContractXmlSerializer(), new UserXmlDto{ Age = age, Name = name }, "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<root xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n  <age>" + age + "</age>\r\n  <name>" + name + "</name>\r\n</root>");
            yield return new Tuple<IObjectSerializer, object, string>(new DataContractJsonObjectSerializer(), new UserJsonDto{ Age = age, Name = name }, "{\"age\":" + age + ",\"name\":\"" + name + "\"}");
        }
    }

}
