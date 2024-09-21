# SoftAPIClient
A type-safe REST library for .NET Core 3.1+, net461+ with an object-oriented approach.

| Active Integrations | Status |
|-|-|
| *Github Build* | ![](https://github.com/oleksandr-fomenko/softapi/actions/workflows/build.yml/badge.svg) |
| *SonarCloud* | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=oleksandr-fomenko_softapi&metric=alert_status)](https://sonarcloud.io/dashboard?id=oleksandr-fomenko_softapi) [![Sonar Violations (long format)](https://img.shields.io/sonar/violations/oleksandr-fomenko_softapi?format=long&label=%20&logo=sonarqube&server=https%3A%2F%2Fsonarcloud.io)](https://sonarcloud.io/dashboard?id=oleksandr-fomenko_softapi) |
| *CodeCov* | [![codecov](https://codecov.io/gh/oleksandr-fomenko/softapi/branch/master/graph/badge.svg?token=H3P1OT781H)](https://codecov.io/gh/oleksandr-fomenko/softapi)|
| *FOSSA* | [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Foleksandr-fomenko%2Fsoftapi.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2Foleksandr-fomenko%2Fsoftapi?ref=badge_shield) |
| *Snyk* | [![](https://avatars.githubusercontent.com/ml/251?s=24)](https://app.snyk.io/org/oleksandr-fomenko/projects) |

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
0. Define request/response API models:
```csharp
public class PostmanResponse
{
    [JsonProperty("args")]
    public PostmanArgs Args { get; set; }
    [JsonProperty("headers")]
    public IDictionary<string, string> Headers { get; set; }
}
public class PostmanArgs
{
    [JsonProperty("foo1")]
    public int Foo1 { get; set; }
    [JsonProperty("foo2")]
    public string Foo2 { get; set; }
}
public class CustomDynamicHeader : DynamicParameter
{
    public CustomDynamicHeader(string headerName, string headerValue)
    {
        var currentUtcTimeTicks = DateTime.UtcNow.Ticks;
        var headerNameModified = $"{headerName}-{currentUtcTimeTicks}";
        var headerValueModified = $"{headerValue}-{currentUtcTimeTicks}";

        AttributeType = AttributeType.Header;
        Name = headerNameModified;
        Value = headerValueModified;
    }
}
```
1. Declare interface with API contract:
```csharp
[Client(Url = "https://postman-echo.com")]
public interface IPostmanEchoRequestMethodsService
{
    [RequestMapping(Method.GET, Path = "/get")]
    Func<ResponseGeneric<PostmanResponse>> Get([QueryParameter("foo1")] int foo1, 
        [QueryParameter("foo2")] string foo2,  
        [DynamicParameter] IDynamicParameter foo3Dynamic);
}
```
2. Register at least one converter:
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
        //define input data
        int foo1 = 42;
        string foo2 = "foooooo";
        string headerNamePrefix = "some-header-name";
        string headerValuePrefix = "some-header-value";
        IDynamicParameter foo3Dynamic = new CustomDynamicHeader(headerNamePrefix, headerValuePrefix);
        
        //fire request
        ResponseGeneric<PostmanResponse> response = RestClient.Instance
            .GetService<IPostmanEchoRequestMethodsService>()
            .Get(foo1, foo2, foo3Dynamic)
            .Invoke();
        
        //verify response    
        Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
        PostmanResponse postmanResponseBody = response.Body;
        Assert.AreEqual(foo1, postmanResponseBody.Args.Foo1);
        Assert.AreEqual(foo2, postmanResponseBody.Args.Foo2);
        
        IDictionary<string, string> actualHeaders = postmanResponseBody.Headers;
        Assert.NotNull(actualHeaders);
        Assert.True(actualHeaders.Any(h => h.Key.StartsWith(headerNamePrefix)));
        Assert.True(actualHeaders.Any(h => h.Value.StartsWith(headerValuePrefix)));
    }
}
```
## Features  Overview

### Interface Attributes

SoftAPIClient attributes define the `Contract` between the interface and how to build the `Request`.  SoftAPIClient's default contract defines the following attributes:

| Attribute     | Target |IsMandatory | Usage | 
|----------------|------------------|-------|-------|
| `[Client]` | Interface           | `true`| Identifies that the represented interface might be instantiated by `SoftAPIClient`. Consumes optional `ResponseConverterType` parameter (as an implementation type of `IResponseConverter` interface registered for the client). Allows specifying `Url` as an immutable value or pair of `DynamicUrlKey` (as a key) + `DynamicUrlType` (as an implementation type of `IDynamicUrl` interface) for applying dynamic URL. Uses `Logger` (as an implementation type of `IRestLogger` interface) which overrides the registered logger for the client if specified. |
| `[RequestMapping]` | Method        | `true`  | Consumes HTTP request `Method` of the resource as a mandatory parameter and a set of the optional parameters like relative URL `Path`, static `Headers`, specific `RequestInterceptor`, and `ResponseInterceptors`. |
| `[Log]`       | Method        |`false`| Log message for a specific request before sending the request. It has an option to specify indexes to refer to which args' values they want to inject into the log message. There is an implementation of the `IRestLogger` -> `RestLogger` for which it's possible to define a log level of this message. |
| `[QueryParameter]`       | Parameter       | `false`| Consumes the name (key) of the query parameter for the request. Applied only to primitives.  |
| `[FormDataParameter]`       | Parameter      | `false` | Consumes the name (key) of the form-data parameter for the request. Applied only to primitives.   |
| `[HeaderParameter]`       | Parameter        |`false`| Consumes the name (key) of the header parameter for the request. Applied only to primitives.   |
| `[PathParameter]`       | Parameter        |`false`| Consumes the name (key) of the path parameter for the request which will be replaced in the Path via curly braces. `[PathParameter("pathId")]` -> /path/{pathId}.  Applied only to primitives.  |
| `[ReplaceableParameter]`       | Parameter      |`false`  | Special case of the Path parameter. It consumes the name (key) of the request's URL parameter, which will be replaced in the URL via curly braces. `[ReplaceableParameter("suiteId")]` -> /path/suite_id={suiteId}.  Applied only to primitives. |
| `[DynamicParameter]`       | Parameter       |`false` |  Uses for the cases when the request attribute is a changeable parameter. Consumes `AttributeType` enum as a type of the request parameter if it's known on the compile time. The default value is `AttributeType.Undefined`. Applied only to `IDynamicParameter` object. |
| `[Body]`       | Parameter       |`false` | Consumes `BodyType` enum as an identifier of the body content, default value is `BodyType.Json`. Applied to objects and primitives parameters.  |
| `[Settings]`       | Parameter        |`false`| Defines that the following argument is `DynamicRequestSettings` object. |

### Core Types

| Type     | Target | Usage | 
|----------------|------------------|-------|
| `Response` | Type           | Core response entity of the HTTP request. |
| `ResponseGeneric<T>` | Type        |  The response entity of the HTTP request is parameterized with a specific body type.   |
| `ResponseGeneric2<T,T2>` | Type        |  The response entity of the HTTP request is parameterized by 2 body types.    |
| `Request` | Type        |  Core HTTP request entity, formed from request mapping by `RequestFactory` class.  |
| `IResponseConverter` | Interface        |  Creates obligation on how the `Func<Request>` should be converter to the `Response` object.  |
| `IInterceptor` | Interface        |  Defines which `Request` data should be applied before making the HTTP request.  |
| `IResponseInterceptor` | Interface        |  Defines how `Response` data should be handled after receiving the HTTP response.   |
| `IDynamicUrl` | Interface        | Returns string value of the URL by input string key. |
| `IDynamicParameter` | Interface        |  Defines data of the request dynamic parameter, defined at runtime.  |
| `IResponseDeserializer` | Interface        |   Determines how to convert incoming string response to the specified generic type.  |
| `IRestLogger` | Interface        |  Defines how to log message which is defined in `Log` attribute, log `Request`, and `Response` data. See `RestLogger` as a default implementation of `IRestLogger` interface.   |

### Request/Response Interceptor usages

Request/Response Interceptors provide features to modify request data or handle responses.
There are 2 types of the interceptors which are applied in the next order:
1. `Client` interceptor - utilized for **ALL** methods in the service.
2. `RequestMapping` interceptor - utilized for specific methods.

Please see the example below.
Request/Response interceptors:
```csharp
public class BaseGitHubRequestInterceptor : IInterceptor
{
    public virtual MetaData.Request Intercept()
    {
        return new MetaData.Request
        {
            Headers = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Accept", "application/vnd.github.inertia-preview+json") }
        };
    }
}

public class GitHubAuthenticationRequestInterceptor : BaseGitHubRequestInterceptor
{
    public override MetaData.Request Intercept()
    {
        var baseRequest = base.Intercept();
        baseRequest.Headers.Add(GitHubDataFactory.AuthorizationData.GetValue());
        return baseRequest;
    }
}

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
```
Usages:
```csharp
[Client(DynamicUrlKey = "GitHub:Api", Path = "/repos", DynamicUrlType = typeof(DynamicConfiguration),
    RequestInterceptor = typeof(GitHubAuthenticationRequestInterceptor),
    ResponseInterceptors = new[] { typeof(GitHubAuthorizationResponseInterceptor)} )]
public interface IGitHubRepositoryProjectService
{
    [Log("Send GET request to 'GitHub API' for getting projects list for the owner={0} and repository={1}")]
    [RequestMapping(Method.GET, Path = "/{owner}/{repo}/projects")]
    Func<ResponseGeneric2<List<GitHubResponse>, GitHubErrorResponse>> GetProjects([PathParameter("owner")] string owner, [PathParameter("repo")] string repository);

    [Log("Send GET request to 'GitHub API' for getting current user-data")]
    [RequestMapping(Method.GET, Path = "/{owner}/{repo}/projects", RequestInterceptor = typeof(GitHubRepositoryRequestInterceptor))]
    Func<ResponseGeneric<List<GitHubResponse>>> GetProjects();

    [Log("Send POST request to 'GitHub API' for creating a next project={0}")]
    [RequestMapping(Method.POST, Path = "/{owner}/{repo}/projects", RequestInterceptor = typeof(GitHubRepositoryRequestInterceptor),
        ResponseInterceptors = new[] { typeof(GitHubInternalServerErrorResponseInterceptor), typeof(GitHubServiceUnavailableResponseInterceptor) })]
    Func<ResponseGeneric<GitHubResponse>> CreateRepositoryProject([Body] GitHubBodyRequest body);
}
```
Invocation order of `IGitHubRepositoryProjectService` methods:
**`GetProjects(owner,repo)`**:
1. `GitHubAuthenticationRequestInterceptor`
2. Request call of the method `GetProjects(owner,repo)`
3. `GitHubAuthorizationResponseInterceptor`

**`GetProjects()`**:
1. `GitHubAuthenticationRequestInterceptor`
2. `GitHubRepositoryRequestInterceptor`
3. Request call of the method `GetProjects()`
4. `GitHubAuthorizationResponseInterceptor`

**`CreateRepositoryProject(body)`**:
1. `GitHubAuthenticationRequestInterceptor`
2. `GitHubRepositoryRequestInterceptor`
3. Request call of the method `CreateRepositoryProject(body)`
4. `GitHubAuthorizationResponseInterceptor`
5. `GitHubInternalServerErrorResponseInterceptor`
6. `GitHubServiceUnavailableResponseInterceptor`

### Logging
1. Set a generic Logger which will be applied to all services:
```csharp
[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void OneTimeBaseSetup()
    {
        RestClient.Instance
            .AddResponseConvertor(new RestSharpResponseConverter())
            .SetLogger(new RestLogger(
                    m => log.Info(m),
                    m => log.Debug(m),
                    m => log.Debug(m))
                );
    }
}
```
2. Override the generic Logger for the specific Service. In this case, the represented logger must have a constructor without any parameters!
```csharp
[Client(Url = "https://postman-echo.com", Logger = typeof(CustomRestLogger))]
public interface IPostmanEchoRequestMethodsService
{
    [RequestMapping(Method.GET, Path = "/get")]
    Func<ResponseGeneric<PostmanResponse>> Get([QueryParameter("foo1")] int foo1, 
        [QueryParameter("foo2")] string foo2,  
        [DynamicParameter] IDynamicParameter foo3Dynamic);
}
```

## Examples
For more details, please see [SoftAPIClient.Example](https://github.com/oleksandr-fomenko/softapi/tree/master/SoftAPI/SoftAPIClient.Example) project.

## License
[![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2Foleksandr-fomenko%2Fsoftapi.svg?type=large)](https://app.fossa.com/projects/git%2Bgithub.com%2Foleksandr-fomenko%2Fsoftapi?ref=badge_large)

Give it a Star! :star:

If you liked the project or if SoftAPIClient helped you, please **give it a star**. If you would like to support the author - [![Support me on Patreon](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Fshieldsio-patreon.vercel.app%2Fapi%3Fusername%3Doleksandrfomenko%26type%3Dpatrons&style=flat)](https://patreon.com/oleksandrfomenko)
