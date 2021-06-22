using System.Xml.Serialization;
using NUnit.Framework;

namespace SoftAPIClient.RestSharpNewtonsoft.Tests
{
    public class RestSharpXmlResponseDeserializerTests : AbstractTest
    {

        [Test]
        public void WhenValidStringIsProvidedInstanceIsCreated()
        {
            const int age = 15;
            const string name = "Alex";
            var restSharpXmlResponseDeserializer = new RestSharpXmlResponseDeserializer();
            var result = restSharpXmlResponseDeserializer.Convert<UserXmlDto>("<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><age>" + age + "</age><name>" + name + "</name></root>");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UserXmlDto>(result);
            Assert.AreEqual(age, result.Age);
            Assert.AreEqual(name, result.Name);
        }

    }

    [XmlRoot(ElementName = "root")]
    public class UserXmlDto
    {
        [XmlElement(ElementName = "age")]
        public int Age { get; set; }
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
    }
}
