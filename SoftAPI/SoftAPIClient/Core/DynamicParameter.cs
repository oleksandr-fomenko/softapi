using System.Collections.Generic;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Core
{
    public class DynamicParameter : IDynamicParameter
    {
        protected string Name { get; set; }
        protected string Value { get; set; }
        protected AttributeType AttributeType { get; set; } = AttributeType.Undefined;

        public DynamicParameter()
        {
        }

        public DynamicParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public DynamicParameter(AttributeType attributeType, string name, string value)
        {
            AttributeType = attributeType;
            Name = name;
            Value = value;
        }
        public KeyValuePair<string, string> GetValue()
        {
            return new KeyValuePair<string, string>(Name, Value);
        }

        public override string ToString()
        {
            return $"AttributeType={AttributeType}, {Name}={Value}";
        }

        public AttributeType GetAttributeType()
        {
            return AttributeType;
        }

    }
}
