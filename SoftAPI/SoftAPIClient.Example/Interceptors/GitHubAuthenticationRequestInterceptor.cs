using System.Collections.Generic;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.Example.Factories;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Interceptors
{
    public class GitHubAuthenticationRequestInterceptor : IInterceptor
    {
        public Request Intercept()
        {
            return new Request
            {
                Headers = new List<KeyValuePair<string, string>> { UserDataFactory.AuthorizationData.GetValue() }
            };
        }
    }
}
