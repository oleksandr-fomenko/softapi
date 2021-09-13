using System.Collections.Generic;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.MetaData
{
    public class Response
    {
        public System.Net.HttpStatusCode HttpStatusCode { get; set; }
        public System.Uri ResponseUri { get; set; }
        public List<KeyValuePair<string, string>> Headers { get; set; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, string>> Cookies { get; set; } = new List<KeyValuePair<string, string>>();
        public string ContentType { get; set; }
        public Request OriginalRequest { get; set; }
        public object OriginalResponse { get; set; }
        public string ResponseBodyString { get; set; }
        public long ElapsedTime { get; set; }
        public IResponseDeserializer Deserializer { get; set; }
        public T GetEntity<T>()
        {
            return Deserializer != null ? Deserializer.Convert<T>(ResponseBodyString) : default;
        }
        public override string ToString()
        {
            return ResponseBodyString ?? string.Empty;
        }
    }
}
