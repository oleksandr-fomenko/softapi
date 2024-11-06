﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RestSharp;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.RestSharpNewtonsoft
{
    public class RestSharpResponseConverter : IResponseConverter
    {
        public virtual Response Convert(Func<Request> request)
        {
            var requestObject = request.Invoke();
            var settings = requestObject.Settings;
            IRestClient client = new RestClient(requestObject.Url)
            {
                FollowRedirects = settings?.FollowRedirects ?? true
            };

            if (settings?.Encoder != null)
            {
                client.UseUrlEncoder(settings.Encoder);
            }

            foreach (var pair in requestObject.PathParameters)
            {
                client.AddDefaultUrlSegment(pair.Key, pair.Value?.ToString());
            }

            foreach (var pair in requestObject.QueryParameters)
            {
                client.AddDefaultQueryParameter(pair.Key, pair.Value?.ToString());
            }

            var restRequest = new RestRequest((Method)Enum.Parse(typeof(Method),requestObject.Method));

            requestObject.Headers.ForEach(h => restRequest.AddHeader(h.Key, h.Value));

            requestObject.FileParameters.ForEach(f => restRequest.AddFile(f.Name, f.Bytes, f.FileName, f.ContentType));

            if (requestObject.FormDataParameters.Count != 0)
            {
                foreach (var pair in requestObject.FormDataParameters)
                {
                    restRequest.AddParameter(pair.Key, pair.Value);
                }
            }

            HandleBody(requestObject, restRequest);

            var deserializer = requestObject.Deserializer ?? new RestSharpJsonResponseDeserializer();

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var response = GetRestResponse(client, restRequest);
            stopWatch.Stop();

            var responseHeaders = response.Headers
                .Select(p => new KeyValuePair<string, string>(p.Name, p.Value?.ToString())).ToList();
            var responseCookies = response.Cookies
                .Select(p => new KeyValuePair<string, string>(p.Name, p.Value?.ToString())).ToList();

            var result = new Response
            {
                HttpStatusCode = response.StatusCode,
                ResponseUri = response.ResponseUri,
                Headers = responseHeaders,
                Cookies = responseCookies,
                ContentType = response.ContentType,
                OriginalRequest = requestObject,
                OriginalResponse = response,
                ResponseBodyString = response.Content,
                ElapsedTime = stopWatch.ElapsedMilliseconds,
                Exception = response.ErrorException,
                Deserializer = deserializer
            };
            return result;
        }

        protected virtual void HandleBody(Request request, IRestRequest restRequest)
        {
            var requestBody = request.Body.Value;
            if (requestBody == null)
            {
                return;
            }

            if (request.BodyName != null)
            {
                string contentType = null;
                object resultBody = null;
                switch (request.Body.Key)
                {
                    case BodyType.Json:
                        contentType = "application/json";
                        resultBody = new RestSharpJsonSerializer().Convert(requestBody);
                        break;
                    case BodyType.Xml:
                        contentType = "application/xml";
                        resultBody = new RestSharpXmlSerializer().Convert(requestBody);
                        break;
                    case BodyType.Text:
                        contentType = "text/plain";
                        resultBody = requestBody;
                        break;
                }
                restRequest.AddParameter(request.BodyName, resultBody, contentType, ParameterType.RequestBody);
            }
            else
            {
                switch (request.Body.Key)
                {
                    case BodyType.Json:
                        var serializedBody = new RestSharpJsonSerializer().Convert(requestBody);
                        restRequest.AddParameter("application/json", serializedBody, ParameterType.RequestBody);
                        restRequest.AddJsonBody(requestBody);
                        break;
                    case BodyType.Xml:
                        restRequest.XmlSerializer = new RestSharp.Serializers.DotNetXmlSerializer();
                        restRequest.RequestFormat = DataFormat.Xml;
                        restRequest.AddXmlBody(requestBody);
                        break;
                    case BodyType.Text:
                        restRequest.AddParameter("text/plain", requestBody, ParameterType.RequestBody);
                        break;
                }
            }
        }

        protected virtual IRestResponse GetRestResponse(IRestClient client, RestRequest restRequest) => client.Execute(restRequest);
    }
}
