using System;
using System.Net;
using SoftAPIClient.Core.Interfaces;

namespace SoftAPIClient.Example.Interceptors.GitHub.Response
{
    public class GitHubInternalServerErrorResponseInterceptor : IResponseInterceptor
    {
        public void ProcessResponse(MetaData.Response response)
        {
            if (HttpStatusCode.InternalServerError == response.HttpStatusCode)
            {
                throw new Exception($"Internal Server Error. Details: {response.ResponseBodyString}");
            }
        }
    }
}