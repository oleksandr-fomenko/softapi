using NUnit.Framework;
using SoftAPIClient.Example.Services.NegativeCases;
using System;
using System.Net;

namespace SoftAPIClient.Example.Tests.NegativeCases
{
    public class NonExistingHostTests : AbstractTest
    {
        [Test]
        public void VerifyNonExistingHostTest()
        {
            var response = GetService<INonExistingHostService>()
                .SomeRequest()
                .Invoke();
            Assert.AreEqual((HttpStatusCode)0, response.HttpStatusCode);
            Assert.NotNull(response.Exception);
            Assert.NotNull(response.Exception.Message);
        }
    }
}
