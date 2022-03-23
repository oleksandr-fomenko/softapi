using System.IO;
using System.Xml.Serialization;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.Implementations
{
    public class DataContractXmlSerializer : IObjectSerializer
    {
        public string Convert(object obj)
        {
            var serializer = new XmlSerializer(obj.GetType());
            var sw = new StringWriter();
            serializer.Serialize(sw, obj);
            return sw.ToString();
        }
    }
}
