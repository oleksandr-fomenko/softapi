using NUnit.Framework;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SoftAPIClient.Tests
{
    public class ResponseTests : AbstractTest
    {

        [Test]
        public void VerifyResponse()
        {
            const HttpStatusCode httpStatusCode = HttpStatusCode.OK;
            var responseUri = new Uri("http://localhost:8080");
            var headers = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Content-Type", "application/json") };
            var cookies = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Cookie1", "CookiesValue") };
            const string contentType = "application/json";
            var originalRequest = new Request();
            var originalResponse = new object();
            const string responseBodyString = "{\"name\":\"Ivan\",\"age\":18}";
            const int elapsedTime = 1000;
            IResponseDeserializer deserializer = RequestFactoryTests.GetDeserializer();
            var response = new Response
            {
                HttpStatusCode = httpStatusCode,
                ResponseUri = responseUri,
                Headers = headers,
                Cookies = cookies,
                ContentType = contentType,
                OriginalRequest = originalRequest,
                OriginalResponse = originalResponse,
                ResponseBodyString = responseBodyString,
                ElapsedTime = elapsedTime,
                Deserializer = deserializer
            };

            Assert.AreEqual(httpStatusCode, response.HttpStatusCode);
            Assert.AreEqual(responseUri, response.ResponseUri);
            Assert.AreEqual(headers, response.Headers);
            Assert.AreEqual(cookies, response.Cookies);
            Assert.AreEqual(contentType, response.ContentType);
            Assert.AreEqual(originalRequest, response.OriginalRequest);
            Assert.AreEqual(originalResponse, response.OriginalResponse);
            Assert.AreEqual(responseBodyString, response.ResponseBodyString);
            Assert.AreEqual(elapsedTime, response.ElapsedTime);
            Assert.AreEqual(deserializer, response.Deserializer);
            Assert.AreEqual(responseBodyString, response.ToString());
            Assert.IsNull(response.GetEntity<UserJsonDto>());
        }

        [Test]
        public void VerifyDefaultResponse()
        {
            var response = new Response();

            Assert.AreEqual((HttpStatusCode)0, response.HttpStatusCode);
            Assert.IsNull(response.ResponseUri);
            Assert.AreEqual(Enumerable.Empty<IList<KeyValuePair<string, string>>>(), response.Headers);
            Assert.AreEqual(Enumerable.Empty<IList<KeyValuePair<string, string>>>(), response.Cookies);
            Assert.IsNull(response.ContentType);
            Assert.IsNull(response.OriginalRequest);
            Assert.IsNull(response.OriginalResponse);
            Assert.IsNull(response.ResponseBodyString);
            Assert.AreEqual(0L, response.ElapsedTime);
            Assert.IsNull(response.Deserializer);
            Assert.AreEqual(string.Empty, response.ToString());
            Assert.IsNull(response.GetEntity<UserJsonDto>());
        }

        [Test]
        public void VerifyResponseGeneric()
        {
            var response = GetResponseTestObject();
            var responseGeneric = new ResponseGeneric<UserJsonDto>(response);

            Assert.AreEqual(response.HttpStatusCode, responseGeneric.HttpStatusCode);
            Assert.AreEqual(response.ResponseUri, responseGeneric.ResponseUri);
            Assert.AreEqual(response.Headers, responseGeneric.Headers);
            Assert.AreEqual(response.Cookies, responseGeneric.Cookies);
            Assert.AreEqual(response.ContentType, responseGeneric.ContentType);
            Assert.AreEqual(response.OriginalRequest, responseGeneric.OriginalRequest);
            Assert.AreEqual(response.OriginalResponse, responseGeneric.OriginalResponse);
            Assert.AreEqual(response.ResponseBodyString, responseGeneric.ResponseBodyString);
            Assert.AreEqual(response.ElapsedTime, responseGeneric.ElapsedTime);
            Assert.AreEqual(response.Deserializer, responseGeneric.Deserializer);
            Assert.IsNull(responseGeneric.Body);
        }

        [Test]
        public void VerifyResponseGeneric2()
        {
            var response = GetResponseTestObject();
            var responseGeneric2 = new ResponseGeneric2<UserJsonDto, UserJsonDto>(response);

            Assert.AreEqual(response.HttpStatusCode, responseGeneric2.HttpStatusCode);
            Assert.AreEqual(response.ResponseUri, responseGeneric2.ResponseUri);
            Assert.AreEqual(response.Headers, responseGeneric2.Headers);
            Assert.AreEqual(response.Cookies, responseGeneric2.Cookies);
            Assert.AreEqual(response.ContentType, responseGeneric2.ContentType);
            Assert.AreEqual(response.OriginalRequest, responseGeneric2.OriginalRequest);
            Assert.AreEqual(response.OriginalResponse, responseGeneric2.OriginalResponse);
            Assert.AreEqual(response.ResponseBodyString, responseGeneric2.ResponseBodyString);
            Assert.AreEqual(response.ElapsedTime, responseGeneric2.ElapsedTime);
            Assert.AreEqual(response.Deserializer, responseGeneric2.Deserializer);
            Assert.IsNull(responseGeneric2.Body);
            Assert.IsNull(responseGeneric2.Body2);
        }

        private Response GetResponseTestObject()
        {
            return new Response
            {
                HttpStatusCode = HttpStatusCode.OK,
                ResponseUri = new Uri("http://localhost:8080"),
                Headers = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Content-Type", "application/json") },
                Cookies = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Cookie1", "CookiesValue") },
                ContentType = "application/json",
                OriginalRequest = new Request(),
                OriginalResponse = new object(),
                ResponseBodyString = "{\"name\":\"Ivan\",\"age\":18}",
                ElapsedTime = 1000,
                Deserializer = null
            };
        }

        public class UserJsonDto 
        {
            public string Name { get; set; }
            public int Age { get; set; }

            public override string ToString()
            {
                return "{\"name\":\"" + Name + "\",\"age\":" + Age + "}";
            }
        }
    }
}
