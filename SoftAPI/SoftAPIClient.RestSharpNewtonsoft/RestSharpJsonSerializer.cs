using Newtonsoft.Json;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.RestSharpNewtonsoft
{
    public class RestSharpJsonSerializer : IObjectSerializer
    {
        public string Convert(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
