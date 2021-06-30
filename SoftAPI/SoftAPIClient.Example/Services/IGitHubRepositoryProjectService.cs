using SoftAPIClient.Attributes;
using System;
using System.Collections.Generic;
using RestSharp;
using SoftAPIClient.Example.Core;
using SoftAPIClient.Example.Interceptors;
using SoftAPIClient.Example.Models;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Services
{
    [Client(DynamicUrlKey = "GitHub:Api", Path = "/repos", DynamicUrlType = typeof(DynamicConfiguration), RequestInterceptor = typeof(GitHubAuthenticationRequestInterceptor))]
    public interface IGitHubRepositoryProjectService
    {
        [Log("Send GET request to 'GitHub API' for getting projects list for the owner={0} and repository={1}")]
        [RequestMapping(Method.GET, Path = "/{owner}/{repo}/projects")]
        Func<ResponseGeneric2<List<GitHubResponse>, GitHubErrorResponse>> GetProjects([PathParameter("owner")] string owner, [PathParameter("repo")] string repository);

        [Log("Send GET request to 'GitHub API' for getting current user-data")]
        [RequestMapping(Method.GET, Path = "/{owner}/{repo}/projects", RequestInterceptor = typeof(GitHubRepositoryRequestInterceptor))]
        Func<ResponseGeneric<List<GitHubResponse>>> GetProjects();

        [Log("Send POST request to 'GitHub API' for creating a next project={0}")]
        [RequestMapping(Method.POST, Path = "/{owner}/{repo}/projects", RequestInterceptor = typeof(GitHubRepositoryRequestInterceptor))]
        Func<ResponseGeneric<GitHubResponse>> CreateRepositoryProject([Body] GitHubBodyRequest body);
    }
}
