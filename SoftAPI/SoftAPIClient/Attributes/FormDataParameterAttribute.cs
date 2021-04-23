namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FormDataParameterAttribute : Base.BaseParameterAttribute
    {
        public FormDataParameterAttribute(string value) : base(value)
        {
        }
    }
}
