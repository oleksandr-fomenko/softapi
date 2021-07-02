# SoftAPIClient
A type-safe REST library for .NET Core 3.1 witth object oriented approach.

| Active Integrations | Status |
|-|-|
| *Github Build* | [![](https://img.shields.io/github/workflow/status/automation-solutions-set/softapi/Build%20and%20Test)](https://github.com/automation-solutions-set/softapi/actions?query=workflow%3A%22Build+and+Test%22) |
| *SonarCloud* | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=automation-solutions-set_softapi&metric=alert_status)](https://sonarcloud.io/dashboard?id=automation-solutions-set_softapi) [![Sonar Violations (long format)](https://img.shields.io/sonar/violations/automation-solutions-set_softapi?format=long&label=%20&logo=sonarqube&server=https%3A%2F%2Fsonarcloud.io)](https://sonarcloud.io/dashboard?id=automation-solutions-set_softapi) |
| *CodeCov* | [![codecov](https://codecov.io/gh/automation-solutions-set/softapi/branch/master/graph/badge.svg?token=H3P1OT781H)](https://codecov.io/gh/automation-solutions-set/softapi)|
| *FOSSA* | [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fautomation-solutions-set%2Fsoftapi.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Fautomation-solutions-set%2Fsoftapi?ref=badge_shield) |

|Nuget Packages | Statistics |
|-|-|
|*SoftAPIClient*|[![NuGet](https://img.shields.io/nuget/v/SoftAPIClient.svg)](https://www.nuget.org/packages/SoftAPIClient/) ![](https://img.shields.io/nuget/dt/SoftAPIClient)|
|*SoftAPIClient.RestSharpNewtonsoft*|[![NuGet](https://img.shields.io/nuget/v/SoftAPIClient.RestSharpNewtonsoft.svg)](https://www.nuget.org/packages/SoftAPIClient.RestSharpNewtonsoft/) ![](https://img.shields.io/nuget/dt/SoftAPIClient.RestSharpNewtonsoft)|

## Getting Started
Core package:
```
dotnet add package SoftAPIClient
```
Converter  (`SoftAPIClient.RestSharpNewtonsoft`: [RestSharp](https://github.com/restsharp/RestSharp) + [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)):
```
dotnet add package SoftAPIClient.RestSharpNewtonsoft
```
## Usage
1. Declare interface with API contract:
```csharp
[Client(Url = "https://postman-echo.com")]
public interface IPostmanEchoRequestMethodsService
{
    [RequestMapping(Method.GET, Path = "/get")]
    Func<ResponseGeneric<PostmanResponse>> Get([QueryParameter("foo1")] int foo1, [QueryParameter("foo2")] string foo2);
}
```
2. Register at least one convertor:
```csharp
[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void OneTimeBaseSetup()
    {
        RestClient.Instance
            .AddResponseConvertor(new RestSharpResponseConverter());
    }
}
```
3. Fire your request:
```csharp
public class TestClassExample
{
    [Test]
    public void VerifyGet()
    {
        int foo1 = 42;
        string foo2 = "foooooo";
        var response = RestClient.Instance.GetService<IPostmanEchoRequestMethodsService>()
            .Get(foo1, foo2)
            .Invoke();
        Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
        PostmanResponse postmanResponseBody = response.Body;
        Assert.AreEqual(foo1, postmanResponseBody.Args.Foo1);
        Assert.AreEqual(foo2, postmanResponseBody.Args.Foo2);
    }
}
```
## Features  Overview

### Interface Attributes

SoftAPIClient attributes define the `Contract` between the interface and how to build the `Request`.  SoftAPIClient's default contract defines the next attributes:

| Attribute     | Target |IsMandatory | Usage | 
|----------------|------------------|-------|-------|
| `[Client]` | Interface           | `true`|  |
| `[RequestMapping]` | Method        | `true`  |  |
| `[Log]`       | Method        |`false`|  |
| `[QueryParameter]`       | Parameter       | `false`|  |
| `[FormDataParameter]`       | Parameter      | `false` |  |
| `[HeaderParameter]`       | Parameter        |`false`|  |
| `[PathParameter]`       | Parameter        |`false`|  |
| `[ReplaceableParameter]`       | Parameter      |`false`  |  |
| `[DynamicParameter]`       | Parameter       |`false` |  |
| `[Body]`       | Parameter       |`false` |  |
| `[Settings]`       | Parameter        |`false`|  |

### Core Types

| Type     | Target | Usage | 
|----------------|------------------|-------|
| `Response` | Type           |  |
| `ResponseGeneric<T>` | Type        |     |
| `ResponseGeneric2<T,T2>` | Type        |     |
| `Request` | Type        |     |
| `IResponseConverter` | Interface        |     |
| `IInterceptor` | Interface        |     |
| `IResponseInterceptor` | Interface        |     |
| `IDynamicUrl` | Interface        |     |
| `IDynamicParameter` | Interface        |     |
| `IResponseDeserializer` | Interface        |     |
| `IRestLogger` | Interface        |     |

### Request/Response Interceptor usages

### Logging

## Examples
For more details, please see [SoftAPIClient.Example](https://github.com/automation-solutions-set/softapi/tree/master/SoftAPI/SoftAPIClient.Example) project.

## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Fautomation-solutions-set%2Fsoftapi.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Fautomation-solutions-set%2Fsoftapi?ref=badge_large)

Give a Star! :star:

If you liked the project or if SoftAPIClient helped you, please **give a star**. If you find any problem please open **issue**.