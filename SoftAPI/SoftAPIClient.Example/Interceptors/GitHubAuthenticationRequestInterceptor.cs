using SoftAPIClient.Example.Factories;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Interceptors
{
    public class GitHubAuthenticationRequestInterceptor : BaseGitHubRequestInterceptor
    {
        public override Request Intercept()
        {
            var baseRequest = base.Intercept();
            baseRequest.Headers.Add(GitHubDataFactory.AuthorizationData.GetValue());
            return baseRequest;
        }
    }
}
