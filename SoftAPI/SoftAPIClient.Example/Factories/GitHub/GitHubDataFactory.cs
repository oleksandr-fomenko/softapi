using SoftAPIClient.Core.Auth;
using SoftAPIClient.Example.Core;

namespace SoftAPIClient.Example.Factories.GitHub
{
    public static class GitHubDataFactory
    {
        public static string Username => CustomConfigurationManager.Configuration["GitHub:Username"];
        public static string Token => CustomConfigurationManager.Configuration["GitHub:Token"];
        public static string Plan => CustomConfigurationManager.Configuration["GitHub:Usage_plan"];
        public static AuthBasic64 AuthorizationData => new AuthBasic64(Username, Token);
        public static string OrganizationOwner => CustomConfigurationManager.Configuration["GitHub:Organization:Owner"];
        public static string OrganizationRepository => CustomConfigurationManager.Configuration["GitHub:Organization:Repository"];
    }
}
