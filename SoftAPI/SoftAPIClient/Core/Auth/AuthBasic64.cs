using SoftAPIClient.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftAPIClient.Core.Auth
{
    public class AuthBasic64 : IAuthentication
    {
        private string UserName { get; }
        private string Password { get; }
        public AuthBasic64(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }

        public KeyValuePair<string, string> GetHeader()
        {
            string key = "Authorization";
            string value = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(UserName + ":" + Password));
            return new KeyValuePair<string, string>(key, value);
        }
    }
}
