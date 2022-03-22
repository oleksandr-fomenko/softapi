using RestSharp.Serializers;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.RestSharpNewtonsoft
{
    public class RestSharpXmlSerializer : IObjectSerializer
    {
        public string Convert(object obj)
        {
            return new DotNetXmlSerializer().Serialize(obj);
        }
    }
}
