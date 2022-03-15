using System.Collections.Generic;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.Example.Factories.GitHub;

namespace SoftAPIClient.Example.Interceptors.GitHub.Request
{
    public class GitHubRepositoryRequestInterceptor : IInterceptor
    {
        public MetaData.Request Intercept()
        {
            return new MetaData.Request
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
