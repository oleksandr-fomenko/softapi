using System.Net;
using NUnit.Framework;
using SoftAPIClient.Example.Factories;
using SoftAPIClient.Example.Services;

namespace SoftAPIClient.Example.Tests
{
    public class GitHubUserServiceTests : AbstractTest
    {
        [Test]
        public void VerifyAttributeType()
        {
            var response = GetService<IGitHubUserService>()
                .GetCurrentUser(UserDataFactory.AuthorizationData)
                .Invoke();
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            var body = response.Body;
            Assert.AreEqual(UserDataFactory.Username, body.Login);
            Assert.IsNotNull(body.Plan);
            Assert.AreEqual(UserDataFactory.Plan, body.Plan.Name);
        }
    }
}
