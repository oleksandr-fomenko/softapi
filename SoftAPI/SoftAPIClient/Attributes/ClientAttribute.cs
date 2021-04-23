using SoftAPIClient.Implementations.RestSharpImpl;

namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class ClientAttribute : Base.BaseAttribute
    {
        public System.Type ResponseConverterType { get; } = typeof(RestSharpResponseConverter);
        public string Url { get; set; }
        public string DynamicUrlKey { get; set; } = string.Empty;
        public System.Type Logger { get; set; }
        public System.Type DynamicUrlType { get; set; }

        public ClientAttribute()
        {
        }

        public ClientAttribute(System.Type responseConverterType)
        {
            ResponseConverterType = responseConverterType;
        }

    }
}
