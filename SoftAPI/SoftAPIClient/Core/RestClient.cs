using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core.Exceptions;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;
using MassiveDynamicProxyGenerator;
using MassiveDynamicProxyGenerator.SimpleInjector;
using SimpleInjector;
using IInterceptor = SoftAPIClient.Core.Interfaces.IInterceptor;

namespace SoftAPIClient.Core
{
    public class RestClient
    {
        private static readonly object Padlock = new object();
        private static RestClient _customClient;
        private static IDictionary<Type, IResponseConverter> _responseConvertors;
        private IRestLogger _logger;

        private RestClient()
        {
        }

        public static RestClient Instance
        {
            get
            {
                if (_customClient == null)
                {
                    lock (Padlock)
                    {
                        if (_customClient == null)
                        {
                            _customClient = new RestClient();
                            _responseConvertors = new Dictionary<Type, IResponseConverter>();
                        }
                    }
                }
                return _customClient;
            }
        }

        public RestClient AddResponseConvertor(IResponseConverter responseConvertor)
        {
            var key = responseConvertor.GetType();
            _responseConvertors.TryAdd(key, responseConvertor);
            return this;
        }

        public RestClient SetLogger(IRestLogger logger)
        {
            _logger = logger;
            return this;
        }

        public TService GetService<TService>(IInterceptor additionalInterceptor = null) where TService : class
        {
            var type = GetTypeOfService<TService>();
            var adapter = new InterceptorAdapter(invocation =>
            {
                type.GetMethods().ToList().ForEach(m =>
                {
                    if (IsMethodMatched(invocation, m))
                    {
                        var arguments = invocation.Arguments;
                        var requestFactory = new RequestFactory(type, m, arguments);

                        OverrideLoggerFromClient(requestFactory);

                        Func<Request> request = () => {
                            var resultRequest = requestFactory.BuildRequest(additionalInterceptor);
                            _logger?.LogBefore(GetMessageFromAnnotationIfPresent(m, arguments));
                            _logger?.LogRequest(resultRequest);
                            return resultRequest;
                        };

                        var responseConverter = GetResponseConverter(requestFactory);

                        var returnTypeFromFunc = m.ReturnType.GenericTypeArguments[0];

                        var methodToCall = !returnTypeFromFunc.IsGenericType ? nameof(GetResponse) : nameof(GetResponseGeneric);
                        var methodInfoResponse = typeof(RestClient).GetMethod(methodToCall, BindingFlags.NonPublic | BindingFlags.Instance);

                        var responseMethodArguments = new Expression[] { Expression.Constant(responseConverter), Expression.Constant(request), Expression.Constant(m) }.ToList();

                        var responseInterceptorExpression = GetResponseInterceptorsExpression(requestFactory);

                        responseMethodArguments.Add(responseInterceptorExpression);
                        var callResponse = Expression.Call(Expression.Constant(this), methodInfoResponse, responseMethodArguments.ToArray());
                        var castToFuncResponse = Expression.Convert(callResponse, returnTypeFromFunc);
                        invocation.ReturnValue = Expression.Lambda(castToFuncResponse).Compile();
                    }
                });
            });
            var container = new Container();
            container.RegisterProxy(type, adapter);
            return container.GetInstance<TService>();
        }

        private dynamic GetResponseGeneric(IResponseConverter responseConverter, Func<Request> request, MethodInfo m, IEnumerable<IResponseInterceptor> responseInterceptors)
        {
            var resultResponse = responseConverter.Convert(request);
            _logger?.LogResponse(resultResponse);
            foreach (var ri in responseInterceptors) 
            {
                ri.ProcessResponse(resultResponse);
            }

            var responseGenericType = m.ReturnType.GenericTypeArguments[0].GetGenericTypeDefinition();
            var returnTypeOfTheResponseGeneric = m.ReturnType.GenericTypeArguments[0].GenericTypeArguments;

            var fromGenericTypeResponseGeneric = responseGenericType.MakeGenericType(returnTypeOfTheResponseGeneric);

            dynamic created = Activator.CreateInstance(fromGenericTypeResponseGeneric, resultResponse);
            return created;
        }

        private dynamic GetResponse(IResponseConverter responseConverter, Func<Request> request, MethodInfo m, IEnumerable<IResponseInterceptor> responseInterceptors)
        {
            var resultResponse = responseConverter.Convert(request);
            _logger?.LogResponse(resultResponse);
            foreach (var ri in responseInterceptors)
            {
                ri.ProcessResponse(resultResponse);
            }

            var actualType = m.ReturnType.GenericTypeArguments[0];
            if (typeof(Response) == actualType)
            {
                return resultResponse;
            }
            dynamic created = Activator.CreateInstance(actualType, resultResponse);
            return created;
        }

        private ConstantExpression GetResponseInterceptorsExpression(RequestFactory requestFactory)
        {
            return Expression.Constant(requestFactory.ResponseInterceptors);
        }

        private Type GetTypeOfService<TService>()
        {
            var type = typeof(TService);
            if (type.GetCustomAttribute<ClientAttribute>() != null)
            {
                return type;
            }
            throw new InitializationException($"Provided type '{type.Name}' must be annotated with '{typeof(ClientAttribute).Name}' attribute");
        }

        private bool IsMethodMatched(IInvocation invocation, MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            var arguments = invocation.Arguments;
            var conditionList = new List<Func<bool>>
            {
                () => invocation.MethodName == methodInfo.Name,
                () => arguments.Length == parameters.Length,
            };

            var baseChecksPassed = conditionList.All(c => c.Invoke());

            if (!baseChecksPassed)
            {
                return false;
            }

            var allParameters = parameters.Select((t, i) => new KeyValuePair<Type, Type>(t?.ParameterType, arguments[i]?.GetType()));
            var allParametersMatched = allParameters.All(p => 
            {
                if (p.Value == null && (Nullable.GetUnderlyingType(p.Key) != null ||  p.Key.IsClass))
                {
                    return true;
                }
                return p.Key == p.Value || p.Key.IsAssignableFrom(p.Value);
            });

            return allParametersMatched;
        }

        private void OverrideLoggerFromClient(RequestFactory requestFactory)
        {
            if (requestFactory.Logger != default)
            {
                _logger = requestFactory.Logger;
            }
        }

        private IResponseConverter GetResponseConverter(RequestFactory requestFactory)
        {
            var responseConvertorType = requestFactory.ResponseConverterType;

            if (responseConvertorType == null)
            {
                var result = _responseConvertors.FirstOrDefault().Value;
                if (result != null)
                {
                    return result;
                }

                throw new InitializationException(
                    $"There is no registered convertors found for the '{typeof(RestClient).Name}'. Please add at least one of it.");
            }

            if (!_responseConvertors.ContainsKey(responseConvertorType))
            {
                throw new InitializationException($"Response converter '{responseConvertorType.Name}' is not registered in the {typeof(RestClient).Name}");
            }
            var responseConverter = _responseConvertors[responseConvertorType];
            return responseConverter;
        }

        private string GetMessageFromAnnotationIfPresent(MemberInfo methodInfo, object[] arguments)
        {
            var logAttr = methodInfo.GetCustomAttribute<LogAttribute>();
            if (logAttr == null)
            {
                return null;
            }
            var message = logAttr.Value;
            try
            {
                var args = arguments?.Select(Utils.HandleToStringIfList).ToArray();
                return string.Format(message, args);
            }
            catch (Exception)
            {
                return message;
            }
        }
    }
}
