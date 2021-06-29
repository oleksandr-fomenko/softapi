using System;
using System.Configuration;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.Example.Core
{
    public class DynamicConfiguration : IDynamicUrl
    {
        public string GetUrl(string key)
        {
           
            return CustomConfigurationManager.Configuration[key];
           // throw new NotImplementedException();
        }
    }
}
