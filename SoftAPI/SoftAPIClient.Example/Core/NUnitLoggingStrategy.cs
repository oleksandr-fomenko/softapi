using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using NUnit.Framework;

namespace SoftAPIClient.Example.Core
{
    public class NUnitLoggingStrategy
    {
        private const string ConsoleConversionPattern = "%level %logger %date{HH:mm:ss,fff} - %message";
        private static readonly Level DefaultLogLevel = Level.All;

        public IAppender GetAppender()
        {
            var layout = new PatternLayout(ConsoleConversionPattern);
            layout.ActivateOptions();
            var appender = new NUnitTestsOutputAppender
            {
                Threshold = DefaultLogLevel,
                Layout = layout
            };
            appender.ActivateOptions();
            return appender;
        }
    }

    public class NUnitTestsOutputAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            var loggingEventString = RenderLoggingEvent(loggingEvent);
            TestContext.Progress.WriteLine(loggingEventString);
        }
    }
}
