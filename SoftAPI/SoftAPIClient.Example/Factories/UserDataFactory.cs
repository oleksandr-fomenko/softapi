using SoftAPIClient.Core.Auth;
using SoftAPIClient.Example.Core;

namespace SoftAPIClient.Example.Factories
{
    public static class UserDataFactory
    {
        public static string Username => CustomConfigurationManager.Configuration["GitHub:Username"];
        public static string Token => CustomConfigurationManager.Configuration["GitHub:Token"];
        public static string Plan => CustomConfigurationManager.Configuration["GitHub:Usage_plan"];
        public static AuthBasic64 AuthorizationData => new AuthBasic64(Username, Token);
    }
}
