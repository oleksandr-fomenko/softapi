using SoftAPIClient.Attributes.Base;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class BodyAttribute : BaseParameterAttribute
    {
        public BodyType BodyType { get; } 
        public BodyAttribute(BodyType bodyType)
        {
            BodyType = bodyType;
        }

        public BodyAttribute() : this(BodyType.Json)
        {
        }
    }
}
