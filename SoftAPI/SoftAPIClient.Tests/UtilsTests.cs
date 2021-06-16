using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Exceptions;
using SoftAPIClient.Core.Interfaces;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.Tests
{
    public class UtilsTests : AbstractTest
    {
        [Test]
        public void VerifyCreateInstanceIfNull()
        {
            var resultOne = Utils.CreateInstanceIfNotNull<ResponseTests.UserDto>(null);
            Assert.IsNull(resultOne);
            var resultTwo = Utils.CreateInstanceIfNotNull<int?>(null);
            Assert.IsNull(resultTwo);
            var resultThree = Utils.CreateInstanceIfNotNull<IRestLogger>(null);
            Assert.IsNull(resultThree);
        }

        [Test]
        public void VerifyCreateInstanceIfNotNull()
        {
            var resultOne = Utils.CreateInstanceIfNotNull<ResponseTests.UserDto>(typeof(ResponseTests.UserDto));
            Assert.IsNotNull(resultOne);
            var resultTwo = Utils.CreateInstanceIfNotNull<int>(typeof(int));
            Assert.IsNotNull(resultTwo);
        }

        [TestCaseSource(nameof(GetTestDataForGetArguments))]
        public void VerifyGetArguments(Tuple<IList<object>, MethodInfo, Type, IEnumerable<KeyValuePair<ParameterInfo, object>>> data)
        {
            var result = Utils.GetArguments(data.Item1, data.Item2, data.Item3).ToList();
            Assert.AreEqual(data.Item4, result);
        }

        [Test]
        public void VerifyGetArgumentsWhenDifferentArgumentsCountIsProvided()
        {
            var arguments = new object[] { "1", "2" };
            var type = typeof(ITestInterface);
            var methodInfo = type.GetMethod("Get");
            var ex = Assert.Throws<InitializationException>(() => Utils.GetArguments(arguments, methodInfo, type));
            Assert.AreEqual($"Argument count '{arguments.Length}' and MethodInfo count '{1}' " +
                            $"is not matched for the method '{methodInfo.Name}' in type '{nameof(ITestInterface)}'", ex.Message);
        }

        private static IEnumerable<Tuple<IList<object>, MethodInfo, Type, IEnumerable<KeyValuePair<ParameterInfo, object>>>> GetTestDataForGetArguments()
        {
            var type = typeof(ITestInterface);
            var methodInfo = type.GetMethod("GetAll");
            yield return new Tuple<IList<object>, MethodInfo, Type, IEnumerable<KeyValuePair<ParameterInfo, object>>>(new List<object>(), methodInfo, type, Enumerable.Empty<KeyValuePair<ParameterInfo, object>>());
            methodInfo = type.GetMethod("Get");
            yield return new Tuple<IList<object>, MethodInfo, Type, IEnumerable<KeyValuePair<ParameterInfo, object>>>(new object[] { "1" }, methodInfo, type, new List<KeyValuePair<ParameterInfo, object>>
                {new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[0], "1" )});
            methodInfo = type.GetMethod("Patch");
            const int firstArg = 1;
            const string secondArg = "token";
            var thirdArg = new DynamicParameter(AttributeType.Header, "x-api-token", "x-value");
            yield return new Tuple<IList<object>, MethodInfo, Type, IEnumerable<KeyValuePair<ParameterInfo, object>>>(new object[] { firstArg, secondArg, thirdArg }, methodInfo, type, new List<KeyValuePair<ParameterInfo, object>>
            {
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[0], firstArg ),
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[1], secondArg ),
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[2], thirdArg )
            });
        }

        [Test]
        public void VerifyGetBaseParameterDataDictionaryWhenEmptyCollectionIsProvided()
        {
            var result = Utils.GetBaseParameterDataDictionary<QueryParameterAttribute>(Enumerable.Empty<KeyValuePair<ParameterInfo, object>>());
            Assert.AreEqual(new Dictionary<string, object>(), result);
        }
    }
}
