using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace SoftAPIClient.Tests.Core
{
    public class Logger
    {
        private const string ConsoleConversionPattern = "%level %logger %date{HH:mm:ss,fff} - %message";
        private static readonly Level DefaultLogLevel = Level.All;

        public static void ConfigureConsoleAppender()
        {
            var consoleAppender = GetConsoleAppender();
            BasicConfigurator.Configure(consoleAppender);
            ((Hierarchy)LogManager.GetRepository()).Root.Level = DefaultLogLevel;
        }

        private static IAppender GetConsoleAppender()
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
}
