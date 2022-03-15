using System;
using System.Net;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.Example.Interceptors.GitHub.Response
{
    public class GitHubAuthorizationResponseInterceptor : IResponseInterceptor
    {
        public void ProcessResponse(MetaData.Response response)
        {
            if (HttpStatusCode.Unauthorized == response.HttpStatusCode)
            {
                throw new Exception($"Provided authorization data is invalid. Details: {response.ResponseBodyString}");
            }
        }
    }
}