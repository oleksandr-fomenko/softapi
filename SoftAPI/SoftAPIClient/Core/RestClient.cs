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
            Type key = responseConvertor.GetType();
            _responseConvertors.TryAdd(key, responseConvertor);
            return this;
        }

        public RestClient SetLogger(IRestLogger logger)
        {
            _logger = logger;
            return this;
        }

        public TService GetService<TService>() where TService : class
        {
            var type = GetTypeOfService<TService>();
            var adapter = new InterceptorAdapter(invocation =>
            {
                type.GetMethods().ToList().ForEach(m =>
                {
                    if (invocation.MethodName == m.Name)
                    {
                        var requestFactory = new RequestFactory(type, m, invocation.Arguments);

                        OverrideLoggerFromClient(requestFactory);

                        Func<Request> request = () => {
                            var resultRequest = requestFactory.BuildRequest();
                            _logger?.LogBefore(GetMessageFromAnnotationIfPresent(m, invocation.Arguments));
                            _logger?.LogRequest(resultRequest);
                            return resultRequest;
                        };
                        var responseConvertorType = requestFactory.ResponseConverterType;
                        if (!_responseConvertors.ContainsKey(responseConvertorType))
                        {
                            throw new InitializationException($"Response converter '{responseConvertorType.Name}' is not registered in the {typeof(RestClient).Name}");
                        }
                        var responseConverter = _responseConvertors[responseConvertorType];

                        MethodInfo methodInfoResponse;

                        var returnTypeFromFunc = m.ReturnType.GenericTypeArguments[0];
                        if (!returnTypeFromFunc.IsGenericType)
                        {
                            methodInfoResponse = typeof(RestClient).GetMethod(nameof(GetResponse), BindingFlags.NonPublic | BindingFlags.Instance);
                        }
                        else
                        {
                            methodInfoResponse = typeof(RestClient).GetMethod(nameof(GetResponseGeneric), BindingFlags.NonPublic | BindingFlags.Instance);
                        }

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
            Type type = typeof(TService);
            if (type.IsInterface && type.GetCustomAttribute<ClientAttribute>() != null)
            {
                return type;
            }
            throw new InitializationException($"Provided type '{type.Name}' must be an interface and annotated with '{typeof(ClientAttribute).Name}' attribute");
        }

        private void OverrideLoggerFromClient(RequestFactory requestFactory)
        {
            if (_logger == null)
            {
                _logger = requestFactory.Logger;
            }
        }

        private string GetMessageFromAnnotationIfPresent(MethodInfo methodInfo, object[] arguments)
        {
            var logAttr = methodInfo.GetCustomAttribute<LogAttribute>();
            if (logAttr == null)
            {
                return null;
            }
            var message = logAttr.Value;
            try
            {
                return string.Format(message, arguments);
            }
            catch (Exception)
            {
                return message;
            }
        }
    }
}
