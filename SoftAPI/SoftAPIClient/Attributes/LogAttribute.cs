namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class LogAttribute : Base.BaseParameterAttribute
    {
        public LogAttribute(string value) : base(value)
        {
        }
    }
}
