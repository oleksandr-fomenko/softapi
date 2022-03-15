using System;
using System.Net;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.Example.Interceptors.GitHub.Response
{
    public class GitHubServiceUnavailableResponseInterceptor : IResponseInterceptor
    {
        public void ProcessResponse(MetaData.Response response)
        {
            if (HttpStatusCode.ServiceUnavailable == response.HttpStatusCode)
            {
                throw new Exception($"Service Unavailable. Details: {response.ResponseBodyString}");
            }
        }
    }
}