using SoftAPIClient.Attributes.Base;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class DynamicParameterAttribute : BaseParameterAttribute
    {
        public AttributeType AttributeType { get; } = AttributeType.Undefined;

        public DynamicParameterAttribute()
        {
        }
        public DynamicParameterAttribute(AttributeType attributeType)
        {
            AttributeType = attributeType;
        }
    }
}
