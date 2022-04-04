using System;
using System.Collections.Generic;
using System.Text;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Implementations
{
    public class RestLogger : IRestLogger
    {
        private readonly Action<string> _logBefore;
        private readonly Action<string> _logRequest;
        private readonly Action<string> _logResponse;

        public RestLogger(Action<string> logBefore, Action<string> logRequest, Action<string> logResponse)
        {
            _logBefore = logBefore;
            _logRequest = logRequest;
            _logResponse = logResponse;
        }

        public void LogBefore(string message)
        {
            if (message != null)
            {
                _logBefore.Invoke(message);
            }
        }

        public void LogRequest(Request request)
        {
            var message = new StringBuilder();
            message.AppendLine("--- Request Data start ---");
            message.AppendLine($"Url - '{request.Url}'");
            message.AppendLine($"Method - '{request.Method}'");
            message.Append(GetParamsFormatted(request.QueryParameters, "Query Parameters:"));
            message.Append(GetParamsFormatted(request.PathParameters, "Path Parameters:"));
            message.Append(GetParamsFormatted(request.FormDataParameters, "FormData Parameters:"));
            message.Append(GetFormattedPairList(request.Headers, "Headers:"));
            if (request.Settings != null)
            {
                message.AppendLine();
                message.AppendLine("Dynamic request settings:");
                message.AppendLine();
                message.AppendLine($"{request.Settings}");
                message.AppendLine();
            }

            var bodyValue = request.Body.Value;
            if (bodyValue != null)
            {
                message.AppendLine($"Body: Type - '{request.Body.Key};, Value:");
                message.AppendLine();
                message.AppendLine(Utils.HandleToStringIfList(bodyValue));
            }
            message.AppendLine();
            message.AppendLine("--- Request Data end ---");
            _logRequest.Invoke(message.ToString());
        }

        public void LogResponse(Response response)
        {
            var message = new StringBuilder();
            message.AppendLine("--- Response Data start ---");
            message.AppendLine($"Url - '{response.ResponseUri}'");
            message.AppendLine($"Http Status Code - '{response.HttpStatusCode}'");
            message.AppendLine($"Content Type - '{response.ContentType}'");
            message.Append(GetFormattedPairList(response.Headers, "Headers:"));
            message.Append(GetFormattedPairList(response.Cookies, "Cookies:"));
            if (response.ResponseBodyString != null)
            {
                message.AppendLine();
                message.AppendLine("Response Body string:");
                message.AppendLine(response.ResponseBodyString);
                message.AppendLine();
            }
            message.AppendLine($"Elapsed time, ms - '{response.ElapsedTime}'");
            message.AppendLine();
            message.AppendLine("--- Response Data end ---");
            _logResponse.Invoke(message.ToString());
        }

        private string GetFormattedPairList(List<KeyValuePair<string, string>> pairs, string header)
        {
            var message = new StringBuilder();
            if (pairs.Count != 0)
            {
                message.AppendLine();
                message.AppendLine(header);
                message.AppendLine();
            }
            pairs.ForEach(kv => message.AppendLine(kv.Key + "=" + kv.Value));
            return message.ToString();
        }

        private string GetParamsFormatted(Dictionary<string, object> parameters, string messageHeader)
        {
            var message = new StringBuilder();
            if (parameters.Count != 0)
            {
                message.AppendLine();
                message.AppendLine(messageHeader);
                message.AppendLine();
            }
            foreach (var pair in parameters)
            {
                message.AppendLine(pair.Key + "=" + pair.Value);
            }
            return message.ToString();
        }
    }
}
