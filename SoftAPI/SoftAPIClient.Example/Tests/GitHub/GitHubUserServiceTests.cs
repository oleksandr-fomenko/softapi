using System.Net;
using NUnit.Framework;
using SoftAPIClient.Example.Factories.GitHub;
using SoftAPIClient.Example.Services.GitHub;

namespace SoftAPIClient.Example.Tests.GitHub
{
    public class GitHubUserServiceTests : AbstractTest
    {
        [Test]
        public void VerifyGetCurrentUserData()
        {
            var response = GetService<IGitHubUserService>()
                .GetCurrentUser(GitHubDataFactory.AuthorizationData)
                .Invoke();
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            var body = response.Body;
            Assert.AreEqual(GitHubDataFactory.Username, body.Login);
            Assert.IsNotNull(body.Plan);
            Assert.AreEqual(GitHubDataFactory.Plan, body.Plan.Name);
        }
    }
}
