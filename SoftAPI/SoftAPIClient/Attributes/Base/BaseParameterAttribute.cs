namespace SoftAPIClient.Attributes.Base
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public abstract class BaseParameterAttribute : System.Attribute
    {
        public string Value { get; set; }

        protected BaseParameterAttribute()
        {
        }
        protected BaseParameterAttribute(string value)
        {
            Value = value;
        }
    }
}
