using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Exceptions;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;
using System;
using System.Linq;

namespace SoftAPIClient.Tests
{
    public class RequestFactoryTests : AbstractTest
    {
        [Test]
        public void VerifyInitializationExceptionWhenNullUrlProvided()
        {
            var targetInterface = typeof(ITestInterface);
            const string methodName = "Get";
            var arguments = new []{ "1" };

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);

            var ex = Assert.Throws<InitializationException>(() => requestFactory.BuildRequest());
            Assert.AreEqual($"The result URL is not defined at the interface '{nameof(ITestInterface)}' and method '{methodName}'", ex.Message);
        }

        [Test]
        public void VerifyInitializationExceptionWhenDifferentArgumentsProvided()
        {
            var targetInterface = typeof(ITestInterface);
            const string methodName = "Get";
            var arguments = new[] { "1" , "2"};

            var ex = Assert.Throws<InitializationException>(() => new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments));
            Assert.AreEqual($"Argument count '{arguments.Length}' and MethodInfo count '{1}' " +
                    $"is not matched for the method '{methodName}' in type '{nameof(ITestInterface)}'", ex.Message);
        }

        [Test]
        public void VerifyDefaultRequestFactoryProperties()
        {
            var targetInterface = typeof(ITestInterface);
            const string methodName = "Get";
            var arguments = new[] { "1" };

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);

            Assert.AreEqual(Enumerable.Empty<IResponseInterceptor>(), requestFactory.ResponseInterceptors);
            Assert.IsNull(requestFactory.ResponseConverterType);
            Assert.IsNull(requestFactory.Logger);
        }
        
    }

    [Client]
    public interface ITestInterface
    {
        [RequestMapping("GET", Path = "/path/all")]
        Func<Response> GetAll();

        [RequestMapping("GET", Path = "/path")]
        Func<Response> Get([QueryParameter("id")] string id);

        [RequestMapping("PATCH", Path = "/path/{pathId}")]
        Func<Response> Patch([PathParameter("pathId")] int pathId, [HeaderParameter("Authorization")] string authorization, [DynamicParameter] IDynamicParameter dynamicParameter);
    }
}
