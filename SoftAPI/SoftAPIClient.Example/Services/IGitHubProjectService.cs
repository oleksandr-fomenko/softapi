using SoftAPIClient.Attributes;
using System;
using RestSharp;
using SoftAPIClient.Example.Core;
using SoftAPIClient.Example.Interceptors;
using SoftAPIClient.Example.Models;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Services
{
    [Client(DynamicUrlKey = "GitHub:Api", Path = "/projects", DynamicUrlType = typeof(DynamicConfiguration), RequestInterceptor = typeof(GitHubAuthenticationRequestInterceptor))]
    public interface IGitHubProjectService
    {
        [Log("Send GET request to 'GitHub API' for getting project by Id={0}")]
        [RequestMapping(Method.GET, Path = "/{projectId}")]
        Func<ResponseGeneric2<GitHubResponse, GitHubErrorResponse>> GetProjectById([PathParameter("projectId")] int projectId);

        [Log("Send DELETE request to 'GitHub API' for removing project by Id={0}")]
        [RequestMapping(Method.DELETE, Path = "/{projectId}")]
        Func<ResponseGeneric<GitHubErrorResponse>> DeleteProjectById([PathParameter("projectId")] int projectId);

        [Log("Send PATCH request to 'GitHub API' for updating project by Id={0} with the next data={1}")]
        [RequestMapping(Method.PATCH, Path = "/{projectId}")]
        Func<ResponseGeneric<GitHubResponse>> UpdateProjectById([PathParameter("projectId")] int projectId, [Body] GitHubBodyRequest body);
    }
}
