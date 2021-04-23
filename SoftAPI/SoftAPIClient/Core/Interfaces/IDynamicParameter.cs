using SoftAPIClient.MetaData;
using System.Collections.Generic;

namespace SoftAPIClient.Core.Interfaces
{
    public interface IDynamicParameter
    {
        AttributeType GetAttributeType();
        KeyValuePair<string, string> GetValue();
    }
}
