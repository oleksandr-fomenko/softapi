namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class ReplaceableParameterAttribute : Base.BaseParameterAttribute
    {
        public ReplaceableParameterAttribute(string value) : base(value)
        {
        }
    }
}
