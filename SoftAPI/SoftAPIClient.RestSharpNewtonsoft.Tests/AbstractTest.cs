using NUnit.Framework;

namespace SoftAPIClient.RestSharpNewtonsoft.Tests
{
    [Parallelizable(ParallelScope.Fixtures)]
    [SetUpFixture]
    public class GlobalSetup
    {
    }

    [TestFixture]
    public abstract class AbstractTest
    {
    }
}