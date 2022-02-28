using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using SoftAPIClient.Attributes;
using SoftAPIClient.Core;
using SoftAPIClient.Core.Auth;
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
            var resultOne = Utils.CreateInstanceIfNotNull<ResponseTests.UserJsonDto>(null);
            Assert.IsNull(resultOne);
            var resultTwo = Utils.CreateInstanceIfNotNull<int?>(null);
            Assert.IsNull(resultTwo);
            var resultThree = Utils.CreateInstanceIfNotNull<IRestLogger>(null);
            Assert.IsNull(resultThree);
        }

        [Test]
        public void VerifyCreateInstanceIfNotNull()
        {
            var resultOne = Utils.CreateInstanceIfNotNull<ResponseTests.UserJsonDto>(typeof(ResponseTests.UserJsonDto));
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
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[0], firstArg),
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[1], secondArg),
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[2], thirdArg)
            });
        }

        [Test]
        public void VerifyGetBaseParameterDataDictionaryWhenEmptyCollectionIsProvided()
        {
            var result = Utils.GetBaseParameterDataDictionary<QueryParameterAttribute>(Enumerable.Empty<KeyValuePair<ParameterInfo, object>>());
            Assert.AreEqual(new Dictionary<string, object>(), result);
        }

        [Test]
        public void VerifyGetBaseParameterDataDictionaryIsFilteredAndMappedWhenValidCollectionIsProvided()
        {

            var type = typeof(ITestInterface);
            var methodInfo = type.GetMethod("Patch");
            const int firstArg = 1;
            const string secondArg = "token";
            var thirdArg = new DynamicParameter(AttributeType.Header, "x-api-token", "x-value");
            var input = new List<KeyValuePair<ParameterInfo, object>>
            {
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[0], firstArg),
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[1], secondArg),
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[2], thirdArg)
            };

            var expectedResultPathParameter = new Dictionary<string, object>
            {
                {"pathId",  firstArg}
            };

            var actualResultPathParameter = Utils.GetBaseParameterDataDictionary<PathParameterAttribute>(input);
            Assert.AreEqual(expectedResultPathParameter, actualResultPathParameter);

            var expectedResultHeaderParameter = new Dictionary<string, object>
            {
                {"Authorization",  secondArg}
            };

            var actualResultHeaderParameter = Utils.GetBaseParameterDataDictionary<HeaderParameterAttribute>(input);
            Assert.AreEqual(expectedResultHeaderParameter, actualResultHeaderParameter);


            var expectedResultEmpty = new Dictionary<string, object>();

            var actualResultReplaceableParameter = Utils.GetBaseParameterDataDictionary<ReplaceableParameterAttribute>(input);
            Assert.AreEqual(expectedResultEmpty, actualResultReplaceableParameter);
        }

        [Test]
        public void VerifyGetBaseParameterDataListWhenEmptyCollectionIsProvided()
        {
            var result = Utils.GetBaseParameterDataList<HeaderParameterAttribute>(Enumerable.Empty<KeyValuePair<ParameterInfo, object>>());
            Assert.AreEqual(Enumerable.Empty<KeyValuePair<string, string>>(), result);
        }


        [Test]
        public void VerifyGetBaseParameterDataListIsFilteredAndMappedWhenValidCollectionIsProvided()
        {
            var type = typeof(ITestInterface);
            var methodInfo = type.GetMethod("Patch");
            const int firstArg = 1;
            const string secondArg = "token";
            var thirdArg = new DynamicParameter(AttributeType.Header, "x-api-token", "x-value");
            var input = new List<KeyValuePair<ParameterInfo, object>>
            {
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[0], firstArg),
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[1], secondArg),
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[2], thirdArg)
            };

            var expectedResultHeaderParameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",  secondArg)
            };

            var actualResultHeaderParameter = Utils.GetBaseParameterDataList<HeaderParameterAttribute>(input);
            Assert.AreEqual(expectedResultHeaderParameter, actualResultHeaderParameter);


            var expectedResultEmpty = Enumerable.Empty<KeyValuePair<string, string>>();

            var actualResultReplaceableParameter = Utils.GetBaseParameterDataList<ReplaceableParameterAttribute>(input);
            Assert.AreEqual(expectedResultEmpty, actualResultReplaceableParameter);
        }

        [Test]
        public void VerifyGetBaseParameterDataListWhenNullableArgumentsAreProvided()
        {
            var type = typeof(ITestInterface);
            var methodInfo = type.GetMethod("Patch");
            const int firstArg = 1;
            const string secondArg = null;
            var thirdArg = new DynamicParameter(AttributeType.Header, "x-api-token", null);
            var input = new List<KeyValuePair<ParameterInfo, object>>
            {
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[0], firstArg),
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[1], secondArg),
                new KeyValuePair<ParameterInfo, object>(methodInfo.GetParameters()[2], thirdArg)
            };

            var expectedResultHeaderParameter = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Authorization",  secondArg)
            };

            var actualResultHeaderParameter = Utils.GetBaseParameterDataList<HeaderParameterAttribute>(input);
            Assert.AreEqual(expectedResultHeaderParameter, actualResultHeaderParameter);
        }

        [Test]
        public void VerifyMergeDictionariesWhenEmptyDictionaryIsProvided()
        {
            var input = new Dictionary<string, object>();
            var result = new Dictionary<string, object>();
            
            Utils.MergeDictionaries(result, input);
            Assert.AreEqual(ImmutableDictionary<string, object>.Empty, result);
        }

        [Test]
        public void VerifyMergeDictionariesWhenValidDataIsProvided()
        {
            const string duplicateKey = "duplicate";
            const string keyThree = "settings";
            const string keyFour = "authBasic64";
            var objectOne = new object();
            var objectTwo = new ResponseTests.UserJsonDto();
            var objectThree = new DynamicRequestSettings();
            var objectFour = new AuthBasic64("Ivan", "Password");
            var input = new Dictionary<string, object>
            {
                {duplicateKey,  objectTwo},
                {keyFour,  objectFour},
            };
            var result = new Dictionary<string, object>
            {
                {duplicateKey,  objectOne},
                {keyThree,  objectThree}
            };

            var expectedDictionary = new Dictionary<string, object>
            {
                {duplicateKey,  objectTwo},
                {keyThree,  objectThree},
                {keyFour,  objectFour}
            };

            Utils.MergeDictionaries(result, input);
            Assert.AreEqual(expectedDictionary, result);
        }

        [Test]
        public void VerifyRemoveNullableValuesDictionaryEmpty()
        {
            var result = Utils.RemoveNullableValues(ImmutableDictionary<string, object>.Empty);
            Assert.AreEqual(ImmutableDictionary<string, object>.Empty, result);
        }

        [Test]
        public void VerifyRemoveNullableValuesDictionary()
        {
            const string keyOne = "1";
            const string keyTwo = "2";
            const string keyThree = "3";
            var objectTwo = new ResponseTests.UserJsonDto();

            var input = new Dictionary<string, object>
            {
                {keyOne,  null},
                {keyTwo,  objectTwo},
                {keyThree,  null}
            };

            var expectedDictionary = new Dictionary<string, object>
            {
                {keyTwo,  objectTwo}
            };

            var result = Utils.RemoveNullableValues(input);
            Assert.AreEqual(expectedDictionary, result);
        }

        [Test]
        public void VerifyRemoveNullableValuesListEmpty()
        {
            var result = Utils.RemoveNullableValues(Enumerable.Empty<KeyValuePair<string, string>>());
            Assert.AreEqual(Enumerable.Empty<KeyValuePair<string, string>>(), result);
        }

        [Test]
        public void VerifyRemoveNullableValuesList()
        {
            const string keyOne = "1";
            const string keyTwo = "2";
            const string keyThree = "3";
            const string objectTwo = "token";

            var input = new[]
            {
                new KeyValuePair<string, string>(keyOne,  null),
                new KeyValuePair<string, string>(keyTwo,  objectTwo),
                new KeyValuePair<string, string>(keyThree,  null)
            };

            var expectedList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(keyTwo,  objectTwo)
            };

            var result = Utils.RemoveNullableValues(input);
            Assert.AreEqual(expectedList, result);
        }

        [Test]
        public void VerifyMergeCollectionToListEmpty()
        {
            var input = new List<KeyValuePair<string, string>>();
            Utils.MergeCollectionToList(input, Enumerable.Empty<KeyValuePair<string, string>>());
            Assert.AreEqual(Enumerable.Empty<KeyValuePair<string, string>>(), input);
        }

        [Test]
        public void VerifyMergeCollectionToList()
        {
            const string keyOne = "1";
            const string keyTwo = "2";
            const string keyThree = "3";
            const string valueOne = "token1";
            const string valueTwo = "token2";
            const string valueThree = "token3";

            var input = new[]
            {
                new KeyValuePair<string, string>(keyOne,  valueOne),
                new KeyValuePair<string, string>(keyTwo,  valueTwo)
            };

            var result = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(keyThree,  valueThree)
            };

            var expectedList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(keyThree,  valueThree),
                new KeyValuePair<string, string>(keyOne,  valueOne),
                new KeyValuePair<string, string>(keyTwo,  valueTwo)
            };

            Utils.MergeCollectionToList(result, input); 
            Assert.AreEqual(expectedList, result);
        }

        [Test]
        public void VerifyRemoveDuplicates()
        {
            var input = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("keyUnique",  "valueUnique"),
                new KeyValuePair<string, string>("keyDuplicate1",  "valueUnique1_1"),
                new KeyValuePair<string, string>("KeyDuPlIcAtE1",  "valueUnique1_2"),
                new KeyValuePair<string, string>("keyDuplicate2",  "valueUnique2"),
                new KeyValuePair<string, string>("keyDuplicate1",  "valueUnique1_3"),
            };

            var expectedList = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("keyUnique",  "valueUnique"),
                new KeyValuePair<string, string>("keyDuplicate1",  "valueUnique1_3"),
                new KeyValuePair<string, string>("keyDuplicate2",  "valueUnique2"),
            };

            Utils.RemoveDuplicates(input);
            Assert.AreEqual(expectedList, input);
        }

        [TestCaseSource(nameof(GetTestDataForHandleToStringIfList))]
        public void VerifyHandleToStringIfList(KeyValuePair<object, string> inputData)
        {
            var (input, expectedResult) = inputData;

            var actualResult = Utils.HandleToStringIfList(input);
            Assert.AreEqual(expectedResult, actualResult);
        }

        private static IEnumerable<KeyValuePair<object, string>> GetTestDataForHandleToStringIfList()
        {
            yield return new KeyValuePair<object, string>(null, null);
            yield return new KeyValuePair<object, string>("foo", "foo");
            const string name = ""; 
            const int age = 15;
            var user = new ResponseTests.UserJsonDto {Name = name, Age = age};
            yield return new KeyValuePair<object, string>(user, user.ToString());
            yield return new KeyValuePair<object, string>(new List<object>{ user }, $"[{user}]");
            yield return new KeyValuePair<object, string>(new List<object>{ user, user }, $"[{user},{user}]");
           
            yield return new KeyValuePair<object, string>(new List<object> { new List<object>{ user } }, $"[[{user}]]");
            yield return new KeyValuePair<object, string>(new List<object> { user, new List<object> { user } }, $"[{user},[{user}]]");
            yield return new KeyValuePair<object, string>(new List<object> { new List<object> { user, user }, new List<object> { user } }, $"[[{user},{user}],[{user}]]");
        }
    }
}
