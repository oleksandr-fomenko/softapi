using System.Collections.Generic;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.MetaData
{
    public class Request
    {
        public string Url { get; set; } = default;
        public string Method { get; set; } = default;
        public string Path { get; set; } = string.Empty;
        public Dictionary<string, object> PathParameters { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> QueryParameters { get; set; } = new Dictionary<string, object>();
        public Dictionary<string, object> FormDataParameters { get; set; } = new Dictionary<string, object>();
        public List<KeyValuePair<string, string>> Headers { get; set; } = new List<KeyValuePair<string, string>>();
        public KeyValuePair<BodyType, object> Body { get; set; } = default;
        public IResponseDeserializer Deserializer { get; set; }
        public DynamicRequestSettings Settings { get; set; }

    }
}
