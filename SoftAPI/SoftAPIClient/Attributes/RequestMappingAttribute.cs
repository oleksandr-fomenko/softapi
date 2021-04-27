namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class RequestMappingAttribute : Base.BaseAttribute
    {
        public string Method { get; }
        public string[] Headers { get; set; } = { };
        public RequestMappingAttribute(object method)
        {
            Method = method?.ToString();
        }
    }
}
