using System.Collections.Generic;
using System.Linq;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.MetaData
{
    public class Request : IEqualityComparer<Request>
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
        public List<FileParameter> FileParameters { get; set; } = new List<FileParameter>();

        public bool Equals(Request x, Request y)
        {
            return string.Equals(x.Url, y.Url)
                   && string.Equals(x.Method, y.Method)
                   && string.Equals(x.Path, y.Path)
                   && x.PathParameters.SequenceEqual(y.PathParameters)
                   && x.QueryParameters.SequenceEqual(y.QueryParameters)
                   && x.FormDataParameters.SequenceEqual(y.FormDataParameters)
                   && x.Headers.SequenceEqual(y.Headers)
                   && x.Body.Equals(y.Body)
                   && Equals(x.Settings, y.Settings)
                   && x.FileParameters.SequenceEqual(y.FileParameters);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals(this, (Request)obj);
        }

        public override int GetHashCode()
        {
            return GetHashCode(this);
        }

        public int GetHashCode(Request obj)
        {
            unchecked
            {
                var hashCode = (Url != null ? Url.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Method != null ? Method.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Method != null ? Method.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PathParameters != null ? PathParameters.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (QueryParameters != null ? QueryParameters.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FormDataParameters != null ? FormDataParameters.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Headers != null ? Headers.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Body.GetHashCode();
                hashCode = (hashCode * 397) ^ (Settings != null ? Settings.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FileParameters != null ? FileParameters.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
