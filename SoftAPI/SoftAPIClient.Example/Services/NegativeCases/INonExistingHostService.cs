using System;
using RestSharp;
using SoftAPIClient.Attributes;
using SoftAPIClient.Example.Core;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Services.NegativeCases
{
    [Client(DynamicUrlKey = "NegativeCases:NonExistingHost", DynamicUrlType = typeof(DynamicConfiguration))]
    public interface INonExistingHostService
    {
        [RequestMapping(Method.GET, Path = "/some")]
        Func<Response> SomeRequest();
    }
}
