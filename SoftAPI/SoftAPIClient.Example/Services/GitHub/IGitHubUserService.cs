using System;
using RestSharp;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core.Auth;
using SoftAPIClient.Example.Core;
using SoftAPIClient.Example.Models.GitHub;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Services.GitHub
{
    [Client(DynamicUrlKey = "GitHub:Api", DynamicUrlType = typeof(DynamicConfiguration))]
    public interface IGitHubUserService
    {
        [Log("Send GET request to 'GitHub API' for getting current user-data")]
        [RequestMapping(Method.GET, Path = "/user")]
        Func<ResponseGeneric<GitHubUser>> GetCurrentUser([DynamicParameter] AuthBasic64 authorization);
    }
}
