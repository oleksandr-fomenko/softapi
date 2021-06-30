using System.Collections.Generic;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.Example.Factories.GitHub;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Interceptors.GitHub
{
    public class GitHubRepositoryRequestInterceptor : IInterceptor
    {
        public Request Intercept()
        {
            return new Request
            {
                PathParameters = new Dictionary<string, object>
                {
                    {
                        "owner", GitHubDataFactory.OrganizationOwner
                    },
                    {
                        "repo", GitHubDataFactory.OrganizationRepository
                    },
                }
            };
        }
    }
}
