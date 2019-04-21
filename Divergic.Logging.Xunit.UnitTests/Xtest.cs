namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    public class XTest
    {
        private readonly ITestOutputHelper _output;
        private readonly ILoggerFactory LoggerFactory;

        public XTest(ITestOutputHelper output)
        {
            _output = output;

            var config = new LoggingConfig
            {
                IgnoreTestBoundaryException = true,
                PaddingSpaces = 3
            };
            LoggerFactory = LogFactory.Create(output, config);
        }

    }

    public class MyConfig : LoggingConfig
    {
        public override bool IgnoreTestBoundaryException { get; set; } = true;

        public override string Format(int scopeLevel, string name, LogLevel logLevel, EventId eventId, string message, Exception exception)
        {
            return message;
        }
    }

}