namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using System.Text;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using NSubstitute;

    public class CustomLoggingConfig : LoggingConfig
    {
        public override bool IgnoreTestBoundaryException { get; set; } = true;

        public override string Format(
            int scopeLevel,
            string name,
            LogLevel logLevel,
            EventId eventId,
            string message,
            Exception exception)
        {
            var sb = new StringBuilder();

            if (scopeLevel > 0)
            {
                sb.Append(' ', scopeLevel * 2);
            }

            sb.Append($"{GetShortLogLevelString(logLevel)} ");

            if (!string.IsNullOrEmpty(name))
            {
                sb.Append($"{name} ");
            }

            if (eventId.Id != 0)
            {
                sb.Append($"[{eventId.Id}]: ");
            }

            if (!string.IsNullOrEmpty(message))
            {
                sb.Append(message);
            }

            if (exception != null)
            {
                sb.Append($"\n{exception}");
            }

            return sb.ToString();
        }

        private static string GetShortLogLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace: return "Trace\t";
                case LogLevel.Debug: return "Debug\t";
                case LogLevel.Information: return "Info\t";
                case LogLevel.Warning: return "Warn\t";
                case LogLevel.Error: return "Error\t";
                case LogLevel.Critical: return "Crit\t";
                case LogLevel.None: return "None\t";
                default: throw new Exception("invalid\t");
            }
        }
    }

    public class MyLoggingConfig : LoggingConfig
    {
        public override bool IgnoreTestBoundaryException { get; set; } = true;

        public override string Format(
            int scopeLevel,
            string name,
            LogLevel logLevel,
            EventId eventId,
            string message,
            Exception exception)
        {
            return message;
            //return base.Format(scopeLevel, name, logLevel, eventId, message, exception);
        }
    }

    public class LoggingConfigTests
    {
        [Fact]
        public void CreatesWithIgnoreTestBoundaryExceptionAsFalse()
        {
            var sut = new LoggingConfig();
            sut.IgnoreTestBoundaryException.Should().BeFalse();
        }

        [Fact]
        public void LogWritesMessageUsingSpecifiedLoggingConfig()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var name = Guid.NewGuid().ToString();

            var config = new MyLoggingConfig();
            Assert.Equal(message, config.Format(0, name, logLevel, eventId, message, exception));

            var output = Substitute.For<ITestOutputHelper>();
            var sut = new TestOutputLogger(name, output, config);

            sut.LogError(message);
            output.Received().WriteLine(message);
        }

    }

}