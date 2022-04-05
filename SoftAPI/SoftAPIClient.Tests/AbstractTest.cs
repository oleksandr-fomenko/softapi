using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace SoftAPIClient.Tests
{
    [Parallelizable(ParallelScope.Fixtures)]
    [SetUpFixture]
    public class GlobalSetup
    {
    }

    [TestFixture]
    public abstract class AbstractTest
    {
        protected string GetTestDataFileContent(string fileName)
        {
            var fullFileName = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "StaticData" + Path.DirectorySeparatorChar + fileName;
            if (File.Exists(fullFileName))
            {
                return File.ReadAllText(fullFileName);
            }
            throw new Exception($"Cannot find file by specified pathname: {fullFileName}");
        }

        protected byte[] SerializeToBytes<T>(T e)
        {
            using (var stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, e);
                return stream.GetBuffer();
            }
        }

        protected T DeserializeFromBytes<T>(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return (T)new BinaryFormatter().Deserialize(stream);
            }
        }
    }
}