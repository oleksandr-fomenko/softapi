using System.Runtime.Serialization;
using System.Xml.Serialization;
using NUnit.Framework;
using SoftAPIClient.Implementations;

namespace SoftAPIClient.Tests
{
    public class DeserializerTests : AbstractTest
    {
        [Test]
        public void WhenValidStringIsProvidedXmlInstanceIsCreated()
        {
            const int age = 15;
            const string name = "Alex";
            var xmlDeserializer = new XmlDeserializer();
            var result = xmlDeserializer.Convert<UserXmlDto>("<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><age>" + age + "</age><name>" + name + "</name></root>");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UserXmlDto>(result);
            Assert.AreEqual(age, result.Age);
            Assert.AreEqual(name, result.Name);
        }

        [Test]
        public void WhenValidStringIsProvidedJsonInstanceIsCreated()
        {
            const int age = 14;
            const string name = "Ivan";
            var dataContractJsonDeserializer = new DataContractJsonDeserializer();
            var result = dataContractJsonDeserializer.Convert<UserJsonDto>("{\"name\":\"" + name + "\",\"age\":" + age + "}");

            Assert.IsNotNull(result);
            Assert.IsInstanceOf<UserJsonDto>(result);
            Assert.AreEqual(age, result.Age);
            Assert.AreEqual(name, result.Name);
        }
    }

    [DataContract]
    public class UserJsonDto
    {
        [DataMember(Name = "age")]
        public int Age { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
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
