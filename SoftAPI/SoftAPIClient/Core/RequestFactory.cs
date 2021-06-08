using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core.Exceptions;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Core
{
    public class RequestFactory
    {
        public Type ResponseConverterType => Client.ResponseConverterType;
        private const string SplitCharter = "=";
        private Type InterfaceType { get; }
        private MethodInfo MethodInfo { get; }
        private object[] Arguments { get; }
        private ClientAttribute Client => InterfaceType.GetCustomAttribute<ClientAttribute>();
        private RequestMappingAttribute RequestMapping => MethodInfo.GetCustomAttribute<RequestMappingAttribute>();
        private IEnumerable<KeyValuePair<ParameterInfo, object>> GetArgumentsData()
        {
            IList<ParameterInfo> parameterInfos = MethodInfo.GetParameters();
            IList<object> parameterValues = Arguments;
            if (parameterInfos.Count != parameterValues.Count)
            {
                throw new InitializationException(
                    $"Argument count '{parameterValues.Count}' and MethodInfo count '{parameterInfos.Count}' " +
                    $"is not matched for the method '{MethodInfo.Name}' in type '{InterfaceType.Name}'");
            }
            return parameterInfos.Select((t, i) => new KeyValuePair<ParameterInfo, object>(t, parameterValues[i])).ToList();
        }

        private Dictionary<string, object> PathParameters => GetArgumentsData()
                .Where(pair => pair.Key.GetCustomAttribute<PathParameterAttribute>() != null)
                .ToDictionary(pair => pair.Key.GetCustomAttribute<PathParameterAttribute>().Value,
                    pair => pair.Value);

        private Dictionary<string, object> QueryParameters => GetArgumentsData()
                .Where(pair => pair.Key.GetCustomAttribute<QueryParameterAttribute>() != null)
                .ToDictionary(pair => pair.Key.GetCustomAttribute<QueryParameterAttribute>().Value,
                    pair => pair.Value);

        private Dictionary<string, object> FormDataParameters => GetArgumentsData()
            .Where(pair => pair.Key.GetCustomAttribute<FormDataParameterAttribute>() != null)
            .ToDictionary(pair => pair.Key.GetCustomAttribute<FormDataParameterAttribute>().Value,
                pair => pair.Value);

        private List<KeyValuePair<string, string>> GetReplaceableParameters() => GetArgumentsData()
            .Where(pair => pair.Key.GetCustomAttribute<ReplaceableParameterAttribute>() != null)
            .Select(pair =>
            {
                var key = pair.Key.GetCustomAttribute<ReplaceableParameterAttribute>().Value;
                var value = pair.Value?.ToString();
                return new KeyValuePair<string, string>(key, value);
            }).ToList();

        private List<KeyValuePair<string, string>> GetHeadersParameters() => GetArgumentsData()
            .Where(pair => pair.Key.GetCustomAttribute<HeaderParameterAttribute>() != null)
            .Select(pair =>
            {
                var key = pair.Key.GetCustomAttribute<HeaderParameterAttribute>().Value;
                var value = pair.Value?.ToString();
                return new KeyValuePair<string, string>(key, value);
            }).ToList();


        private List<KeyValuePair<string, string>> GetRequestHeaders() => RequestMapping.Headers
            .Select(h =>
            {
                var splitHeaders = h.Split(SplitCharter);
                if (splitHeaders.Length < 2)
                {
                    throw new InitializationException($"The following header '{h}' is not specified of the format: key{SplitCharter}value");
                }
                var key = splitHeaders[0];
                var value = splitHeaders.Skip(1).Aggregate((first, next) => first + next);
                return new KeyValuePair<string, string>(key, value);
            }).ToList();

        private KeyValuePair<BodyType, object> GetBody()
        {
            IList<KeyValuePair<ParameterInfo, object>> pairs = GetArgumentsData().Where(p => p.Key.GetCustomAttribute<BodyAttribute>() != null).ToList();
            if (pairs.Count == 0)
            {
                return new KeyValuePair<BodyType, object>();
            }
            return pairs.Select(pair => new KeyValuePair<BodyType, object>(pair.Key.GetCustomAttribute<BodyAttribute>().BodyType, pair.Value)).FirstOrDefault();
        }

        private List<KeyValuePair<AttributeType, IDynamicParameter>> DynamicParameters()
        {
            IList<KeyValuePair<ParameterInfo, object>> pairs = GetArgumentsData().Where(p => p.Key.GetCustomAttribute<DynamicParameterAttribute>() != null).ToList();
            if (pairs.Count == 0)
            {
                return new List<KeyValuePair<AttributeType, IDynamicParameter>>();
            }
            return pairs.Select(pair => new KeyValuePair<AttributeType, IDynamicParameter>(pair.Key.GetCustomAttribute<DynamicParameterAttribute>().AttributeType, pair.Value as IDynamicParameter))
                .Select(HandleDynamicParameters)
                .ToList();
        }

        private DynamicRequestSettings Settings
        {
            get
            {
                IList<KeyValuePair<ParameterInfo, object>> pairs = GetArgumentsData().Where(p => p.Key.GetCustomAttribute<SettingsAttribute>() != null).ToList();
                if (pairs.Count == 0)
                {
                    return null;
                }
                return pairs.Select(pair => pair.Value as DynamicRequestSettings).First();
            }
        }

        private IInterceptor ClientInterceptor
        {
            get
            {
                if (Client.RequestInterceptor != null)
                {
                    return (IInterceptor)Activator.CreateInstance(Client.RequestInterceptor);

                }
                return null;
            }
        }

        private IInterceptor RequestInterceptor
        {
            get
            {
                if (RequestMapping.RequestInterceptor != null)
                {
                    return (IInterceptor)Activator.CreateInstance(RequestMapping.RequestInterceptor);

                }
                return null;
            }
        }

        public IEnumerable<IResponseInterceptor> ResponseInterceptors
        {
            get
            {
                var result = Enumerable.Empty<IResponseInterceptor>();
                if (Client.ResponseInterceptors != null)
                {
                    result = result.Concat(Client.ResponseInterceptors.Select(t => (IResponseInterceptor)Activator.CreateInstance(t)));
                }
                if (RequestMapping.ResponseInterceptors != null)
                {
                    result = result.Concat(RequestMapping.ResponseInterceptors.Select(t => (IResponseInterceptor)Activator.CreateInstance(t)));
                }
                return result;
            }
        }

        private string DynamicUrl
        {
            get
            {
                if (Client.DynamicUrlType != null && Client.DynamicUrlKey != string.Empty)
                {
                    return ((IDynamicUrl) Activator.CreateInstance(Client.DynamicUrlType)).GetUrl(Client.DynamicUrlKey);
                }
                return null;
            }
        }

        public IRestLogger Logger
        {
            get
            {
                if (Client.Logger != null)
                {
                    return (IRestLogger)Activator.CreateInstance(Client.Logger);
                }
                return null;
            }
        }

        public RequestFactory(Type interfaceType, MethodInfo methodInfo, object[] arguments)
        {
            InterfaceType = interfaceType;
            MethodInfo = methodInfo;
            Arguments = arguments;
        }

        public Request BuildRequest()
        {
            var clientRequest = ClientInterceptor?.Intercept();
            var specificRequest = RequestInterceptor?.Intercept();
            var dynamicParameterUrl = GetDynamicParameterUrl();
            var resultUrl = DynamicUrl ?? Client.Url ?? dynamicParameterUrl ?? clientRequest?.Url ?? specificRequest?.Url;

            if (resultUrl == null)
            {
                throw new InitializationException($"The result URL is not defined at the interface '{InterfaceType.Name}' and method '{MethodInfo.Name}'");
            }

            resultUrl += Client.Path;
            resultUrl += clientRequest?.Path;
            resultUrl += RequestMapping.Path;
            resultUrl += specificRequest?.Path;
            resultUrl = ApplyReplaceableParameters(resultUrl);

            var resultHeaders = GetRequestHeaders();
            var resultPathParameters = PathParameters;
            var resultQueryParameters = QueryParameters;
            var resultFormDataParameters = FormDataParameters;
            var headerParameters = GetHeadersParameters();

            MergeHeaders(resultHeaders, headerParameters);

            ApplyRequest(clientRequest, resultHeaders, resultPathParameters, resultQueryParameters, resultFormDataParameters);
            ApplyRequest(specificRequest, resultHeaders, resultPathParameters, resultQueryParameters, resultFormDataParameters);

            ApplyDynamicParameters(resultHeaders, resultQueryParameters, resultFormDataParameters);

            var resultDeserializer = clientRequest?.Deserializer ?? specificRequest?.Deserializer;
            return new Request
            {
                Url = resultUrl,
                Method = RequestMapping.Method,
                PathParameters = resultPathParameters,
                QueryParameters = RemoveNullableValues(resultQueryParameters),
                FormDataParameters = RemoveNullableValues(resultFormDataParameters),
                Headers = RemoveNullableValues(resultHeaders.Distinct()),
                Body = GetBody(),
                Deserializer = resultDeserializer,
                Settings = Settings
            };
        }

        private void MergeHeaders(List<KeyValuePair<string, string>> result, IEnumerable<KeyValuePair<string, string>> input)
        {
            result.AddRange(input);
        }

        private void MergeMaps(Dictionary<string, object> result, Dictionary<string, object> input)
        {
            foreach (var (key, value) in input)
            {
                if (!result.ContainsKey(key))
                {
                    result.Add(key, value);
                }
                else
                {
                    result[key] = input[key];
                }
            }
        }

        private void ApplyRequest(Request request,
            List<KeyValuePair<string, string>> resultHeaders,
            Dictionary<string, object> resultPathParameters,
            Dictionary<string, object> resultQueryParameters,
            Dictionary<string, object> resultFormDataParameters)
        {
            if (request == null)
            {
                return;
            }
            MergeHeaders(resultHeaders, request.Headers);
            MergeMaps(resultPathParameters, request.PathParameters);
            MergeMaps(resultQueryParameters, request.QueryParameters);
            MergeMaps(resultFormDataParameters, request.FormDataParameters);
        }

        private Dictionary<string, object> RemoveNullableValues(Dictionary<string, object> input)
        {
            return input.Where(d => d.Value != null).ToDictionary(k => k.Key, v => v.Value);
        }

        private List<KeyValuePair<string, string>> RemoveNullableValues(IEnumerable<KeyValuePair<string, string>> input)
        {
            return input.Where(d => d.Value != null).ToList();
        }

        private void ApplyDynamicParameters(List<KeyValuePair<string, string>> resultHeaders,
            Dictionary<string, object> resultQueryParameters,
            Dictionary<string, object> resultFormDataParameters)
        {
            var dynamicParams = DynamicParameters();
            if (dynamicParams.Count < 1)
            {
                return;
            }
            var headers = dynamicParams
                .Where(pair => pair.Key == AttributeType.Header)
                .Where(pair => pair.Value != null)
                .Select(pair => pair.Value.GetValue());

            var queryParameters = dynamicParams
                .Where(pair => pair.Key == AttributeType.Query)
                .Where(pair => pair.Value != null)
                .Select(pair => pair.Value.GetValue())
                .ToDictionary(k => k.Key, v => v.Value as object);

            var formDataParameters = dynamicParams
                .Where(pair => pair.Key == AttributeType.FormData)
                .Where(pair => pair.Value != null)
                .Select(pair => pair.Value.GetValue())
                .ToDictionary(k => k.Key, v => v.Value as object);

            MergeHeaders(resultHeaders, headers);
            RemoveDuplicateHeaders(resultHeaders);
            MergeMaps(resultQueryParameters, queryParameters);
            MergeMaps(resultFormDataParameters, formDataParameters);
        }

        private KeyValuePair<AttributeType, IDynamicParameter> HandleDynamicParameters(KeyValuePair<AttributeType, IDynamicParameter> input)
        {
            if (input.Key != AttributeType.Undefined)
            {
                return input;
            }
            var value = input.Value;
            if (value == null)
            {
                return input;
            }
            var attributeTypeFromDynamicParameter = value.GetAttributeType();
            if (attributeTypeFromDynamicParameter == AttributeType.Undefined)
            {
                throw new InitializationException("The dynamic attribute-type should be initialized in the attribute or inside the DynamicParameter implementation");
            }
            return new KeyValuePair<AttributeType, IDynamicParameter>(attributeTypeFromDynamicParameter, value);
        }

        private string GetDynamicParameterUrl()
        {
            var dynamicParams = DynamicParameters();
            var isHasUrl = dynamicParams.Any(pair => pair.Key == AttributeType.Url);
            return isHasUrl ? dynamicParams.First().Value.GetValue().Value : null;
        }

        private string ApplyReplaceableParameters(string resultUrl)
        {
            if (GetReplaceableParameters().Any())
            {
                GetReplaceableParameters().ForEach(p =>
                {
                    resultUrl = resultUrl.Replace($"{{{p.Key}}}", p.Value);
                });
            }

            if (DynamicParameters().Any(p => AttributeType.Replaceable.Equals(p.Key)))
            {
                DynamicParameters().ForEach(p =>
                {
                    var (key, value) = p.Value.GetValue();
                    resultUrl = resultUrl.Replace($"{{{key}}}", value);
                });
            }

            return resultUrl;
        }

        private void RemoveDuplicateHeaders(List<KeyValuePair<string, string>> result)
        {
            var tempResult = result.GroupBy(p => p.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Last().Value, StringComparer.OrdinalIgnoreCase).ToList();
            result.Clear();
            result.AddRange(tempResult);
        }
    }
}
