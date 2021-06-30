using System.Collections.Generic;
using System.Linq;
using System.Net;
using NUnit.Framework;
using SoftAPIClient.Example.Factories;
using SoftAPIClient.Example.Services;

namespace SoftAPIClient.Example.Tests
{
    public class GitHubRepositoryProjectReadOnlyTests : AbstractTest
    {
        [Test]
        public void VerifyGetRepositoryProjectListWithDefaultData()
        {
            var response = GetService<IGitHubRepositoryProjectService>()
                .GetProjects()
                .Invoke();

            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            var projectsBody = response.Body;
            Assert.IsNotEmpty(projectsBody);
            Assert.IsTrue(projectsBody.Any(p => GitHubDataFactory.Username.Equals(p.Creator?.Login)));
        }

        [Test]
        public void VerifyGetRepositoryProjectList()
        {
            var response = GetService<IGitHubRepositoryProjectService>()
                .GetProjects(GitHubDataFactory.OrganizationOwner, GitHubDataFactory.OrganizationRepository)
                .Invoke();

            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            var projectsBody = response.Body;
            Assert.IsNotEmpty(projectsBody);
            Assert.IsTrue(projectsBody.Any(p => GitHubDataFactory.Username.Equals(p.Creator?.Login)));
        }

        [TestCaseSource(nameof(GetNonExistedProjectTestData))]
        public void VerifyBadRequestWhenNonExistedRepositoryProjectList(KeyValuePair<string, string> input)
        {
            var owner = input.Key;
            var repository = input.Value;
            var response = GetService<IGitHubRepositoryProjectService>()
                .GetProjects(owner, repository)
                .Invoke();

            Assert.AreEqual(HttpStatusCode.NotFound, response.HttpStatusCode);
            var responseBody = response.Body2;
            Assert.AreEqual("Not Found", responseBody.Message);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetNonExistedProjectTestData()
        {
            yield return new KeyValuePair<string, string>(GitHubDataFactory.OrganizationOwner, "nonExisted");
            yield return new KeyValuePair<string, string>("nonExisted", GitHubDataFactory.OrganizationRepository);
            yield return new KeyValuePair<string, string>("nonExisted", "nonExisted");
        }
    }
}
