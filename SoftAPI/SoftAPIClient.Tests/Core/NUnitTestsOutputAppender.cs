using log4net.Appender;
using log4net.Core;
using NUnit.Framework;

namespace SoftAPIClient.Tests.Core
{
    public class NUnitTestsOutputAppender : AppenderSkeleton
    {

        protected override void Append(LoggingEvent loggingEvent)
        {
            var loggingEventString = RenderLoggingEvent(loggingEvent);
            TestContext.Progress.WriteLine(loggingEventString);
        }
    }
}