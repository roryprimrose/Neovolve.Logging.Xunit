using FluentAssertions;
using Microsoft.Extensions.Logging;
using ModelBuilder;
using Neovolve.UnitTest.Logging;
using System;
using System.Collections.Generic;
using Xunit;

namespace Neovolve.UnitTest.UnitTests.Logging
{
    public class CacheLoggerProviderTests
    {
        [Fact]
        public void CanDisposeMultipleTimesTest()
        {
            var logs = new List<LogEntry>();

            using (var sut = new CacheLoggerProvider(logs))
            {
                Action action = () => sut.Dispose();

                action.Should().NotThrow();
            }
        }

        [Fact]
        public void CreateLoggerReturnsCacheLoggerWithProvidedLogsStorageTest()
        {
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (string message, Exception error) =>
            {
                return data;
            };
            var name = Guid.NewGuid().ToString();
            var logs = new List<LogEntry>();

            using (var sut = new CacheLoggerProvider(logs))
            {
                var logger = sut.CreateLogger(name);

                logger.Should().BeOfType<CacheLogger>();

                logger.Log(LogLevel.Error, eventId, state, exception, formatter);

                logs.Should().HaveCount(1);
                logs[0].EventId.Should().Be(eventId);
                logs[0].Exception.Should().Be(exception);
                logs[0].LogLevel.Should().Be(LogLevel.Error);
                logs[0].State.Should().Be(state);
                logs[0].Message.Should().Be(data);
            }
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullLogsTest()
        {
            Action action = () => new CacheLoggerProvider(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}