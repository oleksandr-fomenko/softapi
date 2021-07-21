using SoftAPIClient.Core;
using SoftAPIClient.MetaData;
using System;

namespace SoftAPIClient.Example.Core
{
    public class CustomDynamicHeader : DynamicParameter
    {
        public CustomDynamicHeader(string headerName, string headerValue)
        {
            var currentUtcTimeTicks = DateTime.UtcNow.Ticks;
            var headerNameModified = $"{headerName}-{currentUtcTimeTicks}";
            var headerValueModified = $"{headerValue}-{currentUtcTimeTicks}";

            AttributeType = AttributeType.Header;
            Name = headerNameModified;
            Value = headerValueModified;
        }
    }
}
