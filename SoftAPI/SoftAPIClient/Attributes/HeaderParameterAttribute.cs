namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class HeaderParameterAttribute : Base.BaseParameterAttribute
    {
        public HeaderParameterAttribute(string value) : base(value)
        {
        }
    }
}
