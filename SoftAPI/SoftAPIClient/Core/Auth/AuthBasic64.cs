using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftAPIClient.Core.Auth
{
    public class AuthBasic64 : IDynamicParameter
    {
        private string UserName { get; }
        private string Password { get; }
        public AuthBasic64(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public AttributeType GetAttributeType()
        {
            return AttributeType.Header;
        }

        public KeyValuePair<string, string> GetValue()
        {
            const string key = "Authorization";
            var value = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(UserName + ":" + Password));
            return new KeyValuePair<string, string>(key, value);
        }
    }
}
