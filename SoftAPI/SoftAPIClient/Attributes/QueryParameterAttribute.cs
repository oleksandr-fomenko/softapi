namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class QueryParameterAttribute : Base.BaseParameterAttribute
    {
        public QueryParameterAttribute(string value) : base(value)
        {
        }
    }
}
