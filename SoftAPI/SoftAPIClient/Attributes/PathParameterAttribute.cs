namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class PathParameterAttribute : Base.BaseParameterAttribute
    {
        public PathParameterAttribute(string value) : base(value)
        {
        }
    }
}
