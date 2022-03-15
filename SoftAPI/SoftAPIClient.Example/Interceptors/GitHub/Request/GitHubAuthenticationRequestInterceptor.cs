using SoftAPIClient.Example.Factories.GitHub;

namespace SoftAPIClient.Example.Interceptors.GitHub.Request
{
    public class GitHubAuthenticationRequestInterceptor : BaseGitHubRequestInterceptor
    {
        public override MetaData.Request Intercept()
        {
            var baseRequest = base.Intercept();
            baseRequest.Headers.Add(GitHubDataFactory.AuthorizationData.GetValue());
            return baseRequest;
        }
    }
}
