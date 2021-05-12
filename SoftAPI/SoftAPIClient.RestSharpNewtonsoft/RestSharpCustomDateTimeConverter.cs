using System.Globalization;
using Newtonsoft.Json.Converters;

namespace SoftAPIClient.RestSharpNewtonsoft
{
    public class RestSharpCustomDateTimeConverter : IsoDateTimeConverter
    {
        public RestSharpCustomDateTimeConverter()
        {
            DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
            DateTimeStyles = DateTimeStyles.AssumeUniversal;
        }
    }
}
