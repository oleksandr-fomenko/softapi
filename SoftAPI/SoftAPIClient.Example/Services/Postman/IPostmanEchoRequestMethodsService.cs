using System;
using RestSharp;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.Example.Core;
using SoftAPIClient.Example.Models.Postman;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Services.Postman
{
    [Client(DynamicUrlKey = "Postman:Api", DynamicUrlType = typeof(DynamicConfiguration))]
    public interface IPostmanEchoRequestMethodsService
    {
        [RequestMapping(Method.GET, Path = "/get")]
        Func<ResponseGeneric<PostmanResponse>> Get([QueryParameter("foo1")] int foo1, [QueryParameter("foo2")] string foo2, [DynamicParameter] IDynamicParameter foo3Dynamic);

        [Log("Send POST request to 'Postman API' for uploading text body={0}")]
        [RequestMapping(Method.POST, Path = "/post")]
        Func<ResponseGeneric<PostmanResponse>> PostStringBody([Body(BodyType.Text)] string body);
    }
}
