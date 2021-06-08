using NUnit.Framework;
using SoftAPIClient.MetaData;

namespace SoftAPIClient.RestSharpNewtonsoft.Tests
{
    public class ResponseTests : AbstractTest
    {

        [Test]
        public void VerifyResponseGetEntity()
        {
            const string name = "Ivan";
            const int age = 18;
            var response = new Response
            {
                ContentType = "application/json",
                ResponseBodyString = "{\"name\":\"" + name + "\",\"age\":" + age + "}",
                Deserializer = new RestSharpJsonResponseDeserializer()
            };
            var entity = response.GetEntity<UserDto>();
            Assert.NotNull(entity);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(age, entity.Age);
        }

        [Test]
        public void VerifyResponseGenericBody()
        {
            const string name = "Ivan";
            const int age = 18;
            var response = new Response
            {
                ContentType = "application/json",
                ResponseBodyString = "{\"name\":\"" + name + "\",\"age\":" + age + "}",
                Deserializer = new RestSharpJsonResponseDeserializer()
            };

            var responseGeneric = new ResponseGeneric<UserDto>(response);
            var body = responseGeneric.Body;
            Assert.NotNull(body);
            Assert.AreEqual(name, body.Name);
            Assert.AreEqual(age, body.Age);
        }

        [Test]
        public void VerifyResponseGeneric2Body()
        {
            const string name = "Ivan";
            const int age = 18;
            var response = new Response
            {
                ContentType = "application/json",
                ResponseBodyString = "{\"name\":\"" + name + "\",\"age\":" + age + "}",
                Deserializer = new RestSharpJsonResponseDeserializer()
            };

            var responseGeneric2 = new ResponseGeneric2<UserDto, UserDto>(response);
            var body1 = responseGeneric2.Body;
            var body2 = responseGeneric2.Body2;

            Assert.NotNull(body1);
            Assert.AreEqual(name, body1.Name);
            Assert.AreEqual(age, body1.Age);

            Assert.NotNull(body2);
            Assert.AreEqual(name, body2.Name);
            Assert.AreEqual(age, body2.Age);
        }

        public class UserDto
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
