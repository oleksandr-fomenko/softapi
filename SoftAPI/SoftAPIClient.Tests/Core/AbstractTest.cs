using System;
using System.Diagnostics;
using System.Threading;
using log4net;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace SoftAPIClient.Tests.Core
{
    [Parallelizable(ParallelScope.Fixtures)]
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void OneTimeBaseSetup()
        {
            Logger.ConfigureConsoleAppender();
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
        }

    }

    [TestFixture]
    public abstract class AbstractTest
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(AbstractTest));
        private readonly ThreadLocal<Stopwatch> _stopWatch = new ThreadLocal<Stopwatch>();

        [SetUp]
        public void BaseTestSetUp()
        {
            _stopWatch.Value = Stopwatch.StartNew();
            Log.Info($"---->  Starting test {TestContext.CurrentContext.Test.MethodName}");
        }

        [TearDown]
        public void BaseTestTearDown()
        {
            _stopWatch.Value?.Stop();
            Log.Info($"---->  Finished test {TestContext.CurrentContext.Test.MethodName} by {_stopWatch.Value?.ElapsedMilliseconds} ms");
            Log.Info($"---->  Test result {Enum.GetName(typeof(TestStatus), TestContext.CurrentContext.Result.Outcome.Status)}");
        }
    }
}