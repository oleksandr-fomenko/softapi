using System.Collections.Generic;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Interceptors
{
    public class BaseGitHubRequestInterceptor : IInterceptor
    {
        public virtual Request Intercept()
        {
            return new Request
            {
                Headers = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Accept", "application/vnd.github.inertia-preview+json") }
            };
        }
    }
}
