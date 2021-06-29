using System;
using System.IO;
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
    }
}