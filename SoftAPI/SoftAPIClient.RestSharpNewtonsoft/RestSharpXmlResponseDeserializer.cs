using System.IO;
using System.Xml.Serialization;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.RestSharpNewtonsoft
{
    public class RestSharpXmlResponseDeserializer : IResponseDeserializer
    {
        public T Convert<T>(string response)
        {
            return (T) new XmlSerializer(typeof(T)).Deserialize(new StringReader(response));
        }
    }
}
