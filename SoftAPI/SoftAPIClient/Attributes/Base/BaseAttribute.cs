namespace SoftAPIClient.Attributes.Base
{
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public abstract class BaseAttribute : System.Attribute
    {
        public System.Type RequestInterceptor { get; set; } = default;
        public System.Type[] ResponseInterceptors { get; set; } = default;
        public string Path { get; set; } = string.Empty;
    }
}
