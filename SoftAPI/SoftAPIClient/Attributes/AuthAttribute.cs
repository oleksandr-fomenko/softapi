using SoftAPIClient.Attributes.Base;

namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class AuthAttribute : BaseParameterAttribute
    {
        public AuthAttribute()
        {
        }
    }
}
