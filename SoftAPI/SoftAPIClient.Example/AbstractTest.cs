using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using SoftAPIClient.Core;
using SoftAPIClient.Example.Core;
using SoftAPIClient.Implementations;
using SoftAPIClient.RestSharpNewtonsoft;

namespace SoftAPIClient.Example
{
    [Parallelizable(ParallelScope.Fixtures)]
    [SetUpFixture]
    public class GlobalSetup
    {
        private static readonly Hierarchy Repository = (Hierarchy)LogManager.GetRepository(Assembly.GetCallingAssembly());
        
        [OneTimeSetUp]
        public void OneTimeBaseSetup()
        {
            BasicConfigurator.Configure(Repository, new NUnitLoggingStrategy().GetAppender());
            var log = LogManager.GetLogger(typeof(RestClient));
            RestClient.Instance
                .AddResponseConvertor(new RestSharpResponseConverter())
                .SetLogger(new RestLogger(
                    m => log.Info(m),
                    m => log.Debug(SensitiveDataHandler.HideSensitiveData(m)),
                    m => log.Debug(SensitiveDataHandler.HideSensitiveData(m)))
                );
        }
    }

    [TestFixture]
    public abstract class AbstractTest
    {
        //just a wrapper for convinced calling
        protected TService GetService<TService>() where TService : class
        {
            return RestClient.Instance.GetService<TService>();
        }
    }
}