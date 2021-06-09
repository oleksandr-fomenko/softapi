using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Exceptions;
using SoftAPIClient.MetaData;
using System;

namespace SoftAPIClient.Tests
{
    public class RestClientTests : AbstractTest
    {
        [Test]
        public void VerifyInitializationExceptionWhenNoClientAttributeProvided()
        {
            var ex = Assert.Throws<InitializationException>(() => RestClient.Instance.GetService<IInterfaceWithoutClientAttribute>().Get("1").Invoke());
            Assert.AreEqual($"Provided type '{typeof(IInterfaceWithoutClientAttribute).Name}' must be annotated with '{typeof(ClientAttribute).Name}' attribute", ex.Message);
        }

        [Test]
        public void VerifyInitializationExceptionWhenNoConvertersProvided()
        {
            var ex = Assert.Throws<InitializationException>(() => RestClient.Instance.GetService<ITestInterface>().Get("1").Invoke());
            Assert.AreEqual($"There is no registered convertors found for the '{typeof(RestClient).Name}'. Please add at least one of it.", ex.Message);
        }

    }

    public interface IInterfaceWithoutClientAttribute
    {
        [RequestMapping("GET", Path = "/path")]
        Func<Response> Get([QueryParameter("id")] string id);
    }
}
