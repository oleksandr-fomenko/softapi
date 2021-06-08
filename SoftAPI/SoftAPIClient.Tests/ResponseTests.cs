using NUnit.Framework;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;
using System;
using System.Collections.Generic;
using System.Net;

namespace SoftAPIClient.Tests
{
    public class ResponseTests : AbstractTest
    {

        [Test]
        public void VerifyResponse()
        {
            var httpStatusCode = HttpStatusCode.OK;
            var responseUri = new Uri("http://localhost:8080");
            var headers = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Content-Type", "application/json") };
            var cookies = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("Cookie1", "CookiesValue") };
            var contentType = "application/json";
            var originalRequest = new Request();
            var originalResponse = new object();
            var responseBodyString = "{\"name\":\"Ivan\",\"age\":18}";
            var elapsedTime = 1000;
            IResponseDeserializer deserializer = null;
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

            Assert.AreEqual(response.HttpStatusCode, httpStatusCode);
            Assert.AreEqual(response.ResponseUri, responseUri);
            Assert.AreEqual(response.Headers, headers);
            Assert.AreEqual(response.Cookies, cookies);
            Assert.AreEqual(response.ContentType, contentType);
            Assert.AreEqual(response.OriginalRequest, originalRequest);
            Assert.AreEqual(response.OriginalResponse, originalResponse);
            Assert.AreEqual(response.ResponseBodyString, responseBodyString);
            Assert.AreEqual(response.ElapsedTime, elapsedTime);
            Assert.AreEqual(response.Deserializer, deserializer);
        }

        [Test]
        public void VerifyResponseGeneric()
        {
            var response = GetResponseTestObject();
            var responseGeneric = new ResponseGeneric<UserDto>(response);

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
        }

        [Test]
        public void VerifyResponseGeneric2()
        {
            var response = GetResponseTestObject();
            var responseGeneric2 = new ResponseGeneric2<UserDto, UserDto>(response);

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

        public class UserDto 
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
