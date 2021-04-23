using System.IO;
using System.Xml.Serialization;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.Implementations.RestSharpImpl
{
    public class RestSharpXmlResponseDeserializer : IResponseDeserializer
    {
        public T Convert<T>(string response)
        {
            var stringReader = new StringReader(response);
            var serializer = new XmlSerializer(typeof(T));
            return (T) serializer.Deserialize(stringReader);
        }
    }
}
