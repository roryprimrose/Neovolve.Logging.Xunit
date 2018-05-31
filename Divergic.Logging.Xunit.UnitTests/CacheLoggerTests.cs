namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using System.Collections.Generic;
    using Divergic.Logging.Xunit;
    using FluentAssertions;
    using global::Xunit;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;

    public class CacheLoggerTests
    {
        [Fact]
        public void CacheLoggerTBeginScopeReturnsInstanceTest()
        {
            var logs = new List<LogEntry>();

            var sut = new CacheLogger<CacheLoggerTests>(logs);

            var actual = sut.BeginScope("Stuff");

            actual.Should().NotBeNull();

            Action action = () => actual.Dispose();

            action.Should().NotThrow();
        }

        [Fact]
        public void CacheLoggerThrowsExceptionWithNullLogsTest()
        {
            Action action = () => new CacheLogger(null);
        }

        [Fact]
        public void CacheLoggerTThrowsExceptionWithNullLogsTest()
        {
            Action action = () => new CacheLogger<CacheLoggerTests>(null);
        }

        [Theory]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Warning)]
        public void IsEnabledReturnsTrueTest(LogLevel logLevel)
        {
            var logs = new List<LogEntry>();

            var sut = new CacheLogger<CacheLoggerTests>(logs);

            var actual = sut.IsEnabled(logLevel);

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Warning)]
        public void LogCachesLogMessageTest(LogLevel logLevel)
        {
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => { return data; };
            var logs = new List<LogEntry>();

            var sut = new CacheLogger<CacheLoggerTests>(logs);

            sut.Log(logLevel, eventId, state, exception, formatter);

            logs.Should().HaveCount(1);
            logs[0].EventId.Should().Be(eventId);
            logs[0].Exception.Should().Be(exception);
            logs[0].LogLevel.Should().Be(logLevel);
            logs[0].State.Should().Be(state);
            logs[0].Message.Should().Be(data);
        }
    }
}