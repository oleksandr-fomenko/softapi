using SoftAPIClient.MetaData;

namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class DynamicParameterAttribute : System.Attribute
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
