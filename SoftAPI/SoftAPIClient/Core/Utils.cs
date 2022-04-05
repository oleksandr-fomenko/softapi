using SoftAPIClient.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using SoftAPIClient.Attributes.Base;
using System.Collections;
using System.Text;

namespace SoftAPIClient.Core
{
    public static class Utils
    {
        public static T CreateInstanceIfNotNull<T>(Type type)
        {
            if (type == null)
            {
                return default;
            }
            return (T)Activator.CreateInstance(type);
        }

        public static IEnumerable<KeyValuePair<ParameterInfo, object>> GetArguments(IList<object> parameterValues, MethodInfo methodInfo, Type interfaceType)
        {
            IList<ParameterInfo> parameterInfos = methodInfo.GetParameters();
            if (parameterInfos.Count != parameterValues.Count)
            {
                throw new InitializationException(
                    $"Argument count '{parameterValues.Count}' and MethodInfo count '{parameterInfos.Count}' " +
                    $"is not matched for the method '{methodInfo.Name}' in type '{interfaceType.Name}'");
            }

            return parameterInfos.Select((t, i) => new KeyValuePair<ParameterInfo, object>(t, parameterValues[i]));
        }

        public static Dictionary<string, object> GetBaseParameterDataDictionary<T>(IEnumerable<KeyValuePair<ParameterInfo, object>> argumentsData) where T : BaseParameterAttribute
        {
            return argumentsData
                .Where(pair => pair.Key.GetCustomAttribute<T>() != null)
                .ToDictionary(pair => pair.Key.GetCustomAttribute<T>().Value,
                    pair => pair.Value);
        }

        public static List<KeyValuePair<string, string>> GetBaseParameterDataList<T>(IEnumerable<KeyValuePair<ParameterInfo, object>> argumentsData) where T : BaseParameterAttribute
        {
            return argumentsData.Where(pair => pair.Key.GetCustomAttribute<T>() != null)
            .Select(pair =>
            {
                var key = pair.Key.GetCustomAttribute<T>().Value;
                var value = pair.Value?.ToString();
                return new KeyValuePair<string, string>(key, value);
            }).ToList();
        }

        public static void MergeDictionaries(Dictionary<string, object> result, Dictionary<string, object> input)
        {
            foreach (var pair in input)
            {
                var key = pair.Key;
                var value = pair.Value;
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

        public static Dictionary<string, object> RemoveNullableValues(IDictionary<string, object> input)
        {
            return input.Where(d => d.Value != null).ToDictionary(k => k.Key, v => v.Value);
        }

        public static List<KeyValuePair<string, string>> RemoveNullableValues(IEnumerable<KeyValuePair<string, string>> input)
        {
            return input.Where(d => d.Value != null).ToList();
        }

        public static void RemoveDuplicates(List<KeyValuePair<string, string>> result)
        {
            var tempResult = result.GroupBy(p => p.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Last().Value, StringComparer.OrdinalIgnoreCase).ToList();
            result.Clear();
            result.AddRange(tempResult);
        }

        public static void MergeCollectionToList(List<KeyValuePair<string, string>> result, IEnumerable<KeyValuePair<string, string>> input)
        {
            result.AddRange(input);
        }

        public static string HandleToStringIfList(object input)
        {
            if (input == null)
            {
                return null;
            }

            if (input is IList enumerable)
            {
                var sb = new StringBuilder("[");
                foreach (var item in enumerable)
                {
                    sb.Append(HandleToStringIfList(item));
                    sb.Append(",");
                }
                var result = sb.ToString();
                return result.Remove(result.Length - 1) + "]";
            }

            return input.ToString();
        }
    }
}
