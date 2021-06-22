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
        private const string HeaderSeparator = "=";
        private Type InterfaceType { get; }
        private MethodInfo MethodInfo { get; }
        private ClientAttribute Client => InterfaceType.GetCustomAttribute<ClientAttribute>();
        private RequestMappingAttribute RequestMapping => MethodInfo.GetCustomAttribute<RequestMappingAttribute>();
        private IEnumerable<KeyValuePair<ParameterInfo, object>> ArgumentsData { get; }
        private Dictionary<string, object> PathParameters => Utils.GetBaseParameterDataDictionary<PathParameterAttribute>(ArgumentsData);
        private Dictionary<string, object> QueryParameters => Utils.GetBaseParameterDataDictionary<QueryParameterAttribute>(ArgumentsData);
        private Dictionary<string, object> FormDataParameters => Utils.GetBaseParameterDataDictionary<FormDataParameterAttribute>(ArgumentsData);
        private List<KeyValuePair<string, string>> GetReplaceableParameters() => Utils.GetBaseParameterDataList<ReplaceableParameterAttribute>(ArgumentsData);
        private List<KeyValuePair<string, string>> GetHeadersParameters() => Utils.GetBaseParameterDataList<HeaderParameterAttribute>(ArgumentsData);

        private List<KeyValuePair<string, string>> GetRequestHeaders() => RequestMapping.Headers
            .Select(h =>
            {
                var splitHeaders = h.Split(HeaderSeparator);
                if (splitHeaders.Length < 2)
                {
                    throw new InitializationException($"The following header '{h}' is not specified of the format: key{HeaderSeparator}value");
                }
                var key = splitHeaders[0];
                var value = splitHeaders.Skip(1).Aggregate((first, next) => first + next);
                return new KeyValuePair<string, string>(key, value);
            }).ToList();

        private KeyValuePair<BodyType, object> GetBody()
        {
            IList<KeyValuePair<ParameterInfo, object>> pairs = ArgumentsData.Where(p => p.Key.GetCustomAttribute<BodyAttribute>() != null).ToList();
            if (pairs.Count == 0)
            {
                return new KeyValuePair<BodyType, object>();
            }
            return pairs.Select(pair => new KeyValuePair<BodyType, object>(pair.Key.GetCustomAttribute<BodyAttribute>().BodyType, pair.Value)).FirstOrDefault();
        }

        private List<KeyValuePair<AttributeType, IDynamicParameter>> DynamicParameters()
        {
            IList<KeyValuePair<ParameterInfo, object>> pairs = ArgumentsData.Where(p => p.Key.GetCustomAttribute<DynamicParameterAttribute>() != null).ToList();
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
                IList<KeyValuePair<ParameterInfo, object>> pairs = ArgumentsData.Where(p => p.Key.GetCustomAttribute<SettingsAttribute>() != null).ToList();
                if (pairs.Count == 0)
                {
                    return null;
                }
                return pairs.Select(pair => pair.Value as DynamicRequestSettings).First();
            }
        }

        private IInterceptor ClientInterceptor => Utils.CreateInstanceIfNotNull<IInterceptor>(Client.RequestInterceptor);
        private IInterceptor RequestInterceptor => Utils.CreateInstanceIfNotNull<IInterceptor>(RequestMapping.RequestInterceptor);

        public IEnumerable<IResponseInterceptor> ResponseInterceptors
        {
            get
            {
                var result = Enumerable.Empty<IResponseInterceptor>();
                if (Client.ResponseInterceptors != null)
                {
                    result = result.Concat(Client.ResponseInterceptors.Select(Utils.CreateInstanceIfNotNull<IResponseInterceptor>));
                }
                if (RequestMapping.ResponseInterceptors != null)
                {
                    result = result.Concat(RequestMapping.ResponseInterceptors.Select(Utils.CreateInstanceIfNotNull<IResponseInterceptor>));
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
                    return Utils.CreateInstanceIfNotNull<IDynamicUrl>(Client.DynamicUrlType).GetUrl(Client.DynamicUrlKey);
                }
                return null;
            }
        }

        public IRestLogger Logger => Utils.CreateInstanceIfNotNull<IRestLogger>(Client.Logger);

        public RequestFactory(Type interfaceType, MethodInfo methodInfo, object[] arguments)
        {
            InterfaceType = interfaceType;
            MethodInfo = methodInfo;
            ArgumentsData = Utils.GetArguments(arguments, methodInfo, interfaceType);
        }

        public Request BuildRequest()
        {
            var clientRequest = ClientInterceptor?.Intercept();
            var specificRequest = RequestInterceptor?.Intercept();

            var resultUrl = GetUrl(clientRequest, specificRequest);
            var resultHeaders = GetRequestHeaders();
            var resultPathParameters = PathParameters;
            var resultQueryParameters = QueryParameters;
            var resultFormDataParameters = FormDataParameters;
            var headerParameters = GetHeadersParameters();

            Utils.MergeCollectionToList(resultHeaders, headerParameters);

            ApplyRequest(clientRequest, resultHeaders, resultPathParameters, resultQueryParameters, resultFormDataParameters);
            ApplyRequest(specificRequest, resultHeaders, resultPathParameters, resultQueryParameters, resultFormDataParameters);

            ApplyDynamicParameters(resultHeaders, resultQueryParameters, resultFormDataParameters);

            var resultDeserializer = clientRequest?.Deserializer ?? specificRequest?.Deserializer;
            return new Request
            {
                Url = resultUrl,
                Method = RequestMapping.Method,
                PathParameters = resultPathParameters,
                QueryParameters = Utils.RemoveNullableValues(resultQueryParameters),
                FormDataParameters = Utils.RemoveNullableValues(resultFormDataParameters),
                Headers = Utils.RemoveNullableValues(resultHeaders.Distinct()),
                Body = GetBody(),
                Deserializer = resultDeserializer,
                Settings = Settings
            };
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
            Utils.MergeCollectionToList(resultHeaders, request.Headers);
            Utils.MergeDictionaries(resultPathParameters, request.PathParameters);
            Utils.MergeDictionaries(resultQueryParameters, request.QueryParameters);
            Utils.MergeDictionaries(resultFormDataParameters, request.FormDataParameters);
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
            var headers = FilterDynamicParameters(dynamicParams, AttributeType.Header);
            var queryParameters = FilterDynamicParameters(dynamicParams, AttributeType.Query).ToDictionary(k => k.Key, v => v.Value as object);
            var formDataParameters = FilterDynamicParameters(dynamicParams, AttributeType.FormData).ToDictionary(k => k.Key, v => v.Value as object);

            Utils.MergeCollectionToList(resultHeaders, headers);
            Utils.RemoveDuplicates(resultHeaders);
            Utils.MergeDictionaries(resultQueryParameters, queryParameters);
            Utils.MergeDictionaries(resultFormDataParameters, formDataParameters);
        }

        private IEnumerable<KeyValuePair<string, string>> FilterDynamicParameters(List<KeyValuePair<AttributeType, IDynamicParameter>> dynamicParams, AttributeType attributeType)
        {
            return dynamicParams.Where(pair => pair.Key == attributeType).Where(pair => pair.Value != null).Select(pair => pair.Value.GetValue());
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

        private string GetUrl(Request clientRequest, Request specificRequest)
        {
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

            return resultUrl;
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
    }
}
