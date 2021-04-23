using System;
using SoftAPIClient.Core.Interfaces;
using Newtonsoft.Json;

namespace SoftAPIClient.Implementations.RestSharpImpl
{
    public class RestSharpJsonResponseDeserializer : IResponseDeserializer
    {
        public T Convert<T>(string response)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(response, new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
            }
            catch (Exception)
            {
                return Activator.CreateInstance<T>();
            }
        }
    }
}
