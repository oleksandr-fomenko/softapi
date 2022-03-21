using SoftAPIClient.MetaData;

namespace SoftAPIClient.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class BodyAttribute : System.Attribute
    {
        public string Name { get; } 
        public BodyType BodyType { get; } 
        public BodyAttribute(BodyType bodyType, string name = null)
        {
            BodyType = bodyType;
            Name = name;
        }

        public BodyAttribute() : this(BodyType.Json)
        {
        }
    }
}
