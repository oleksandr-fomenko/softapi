using System.Collections.Generic;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.Example.Interceptors.GitHub.Request
{
    public class BaseGitHubRequestInterceptor : IInterceptor
    {
        public virtual MetaData.Request Intercept()
        {
            return new MetaData.Request
            {
                Headers = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Accept", "application/vnd.github.inertia-preview+json") }
            };
        }
    }
}
