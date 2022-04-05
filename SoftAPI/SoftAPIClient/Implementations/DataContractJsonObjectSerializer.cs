using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.Implementations
{
    public class DataContractJsonObjectSerializer : IObjectSerializer
    {
        public string Convert(object obj)
        {
            var serializer = new DataContractJsonSerializer(obj.GetType());

            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                var bytes = stream.ToArray();
                return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            }
        }
    }
}
