using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.Implementations
{
    public class DataContractJsonDeserializer : IResponseDeserializer
    {
        public T Convert<T>(string response)
        {
            var settings = new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            };

            var serializer = new DataContractJsonSerializer(typeof(T), settings);

            using var stream = new MemoryStream();
            var bytes = Encoding.UTF8.GetBytes(response);
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;

            return (T)serializer.ReadObject(stream);
        }
    }
}
