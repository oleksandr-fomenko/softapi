using Newtonsoft.Json;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.RestSharpNewtonsoft
{
    public class RestSharpJsonResponseDeserializer : IResponseDeserializer
    {
        public T Convert<T>(string response)
        {
            return JsonConvert.DeserializeObject<T>(response, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
        }
    }
}
