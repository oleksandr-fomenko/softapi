using System;
using RestSharp;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.Example.Core;
using SoftAPIClient.Example.Models.Postman;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Example.Services.Postman
{
    [Client(DynamicUrlKey = "Postman:Api", DynamicUrlType = typeof(DynamicConfiguration))]
    public interface IPostmanEchoHeadersService
    {
        [RequestMapping(Method.GET, Path = "/headers")]
        Func<ResponseGeneric<PostmanResponse>> DynamicParameterByInterface([DynamicParameter] IDynamicParameter dynamicParameter);

        [RequestMapping(Method.GET, Path = "/headers")]
        Func<ResponseGeneric<PostmanResponse>> DynamicParameter([DynamicParameter] DynamicParameter dynamicParameter);

        [RequestMapping(Method.GET, Path = "/headers")]
        Func<ResponseGeneric<PostmanResponse>> DynamicParameterByDefinedAttributeType([DynamicParameter(AttributeType.Header)] DynamicParameter dynamicParameter);

        [RequestMapping(Method.GET, Path = "/headers")]
        Func<ResponseGeneric<PostmanResponse>> DynamicParameterByCustomParameter([DynamicParameter] CustomDynamicHeader customDynamicHeader);
    }
}
