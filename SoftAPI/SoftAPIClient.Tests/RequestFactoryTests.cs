using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Exceptions;
using SoftAPIClient.MetaData;
using System;

namespace SoftAPIClient.Tests
{
    public class RequestFactoryTests : AbstractTest
    {
        [Test]
        public void VerifyNullUrlException()
        {
            var targetInterface = typeof(ITestInterface);
            var methodName = "Get";
            var arguments = new []{ "1" };

            var requestFactory = new RequestFactory(targetInterface, targetInterface.GetMethod(methodName), arguments);

            var ex = Assert.Throws<InitializationException>(() => requestFactory.BuildRequest());
            Assert.AreEqual($"The result URL is not defined at the interface '{nameof(ITestInterface)}' and method '{methodName}'", ex.Message);
        }


        [Client]
        private interface ITestInterface
        {
            [RequestMapping("GET", Path = "/path")]
            Func<Response> Get([QueryParameter("id")] string id);
        }
    }
}
