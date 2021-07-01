using System.Net;
using NUnit.Framework;
using SoftAPIClient.Example.Services.Postman;

namespace SoftAPIClient.Example.Tests.Postman
{
    public class PostmanRequestMethods : AbstractTest
    {
        [Test]
        public void VerifyPostTextBody()
        {
            const string body = "Duis posuere augue vel cursus pharetra.";
            var response = GetService<IPostmanEchoRequestMethodsService>()
                .PostStringBody(body)
                .Invoke();
            Assert.AreEqual(HttpStatusCode.OK, response.HttpStatusCode);
            Assert.AreEqual(body, response.Body.Data);
        }
    }
}
