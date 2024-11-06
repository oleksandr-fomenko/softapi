using NUnit.Framework;
using SoftAPIClient.Example.Services.NegativeCases;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SoftAPIClient.Example.Tests.NegativeCases
{
    public class NonExistingHostTests : AbstractTest
    {
        [Test]
        public void VerifyPostTextBody()
        {
            const string expectedErrorMessage = "No such host is known. No such host is known.";
            var response = GetService<INonExistingHostService>()
                .SomeRequest()
                .Invoke();
            Assert.AreEqual((HttpStatusCode)0, response.HttpStatusCode);
            Assert.NotNull(response.Exception);
            var message = response.Exception.Message;
            Assert.AreEqual(expectedErrorMessage, message);
        }
    }
}
