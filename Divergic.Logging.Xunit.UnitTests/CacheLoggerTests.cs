namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using global::Xunit;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using NSubstitute;

    public class CacheLoggerTests
    {
        [Fact]
        public void BeginScopeReturnsInstanceWhenNoSourceLoggerProvidedTest()
        {
            var state = Guid.NewGuid().ToString();

            var sut = new CacheLogger();

            using (var actual = sut.BeginScope(state))
            {
                actual.Should().NotBeNull();
            }
        }

        [Fact]
        public void BeginScopeReturnsWrappedInstanceWhenSourceLoggerProvidedTest()
        {
            var state = Guid.NewGuid().ToString();

            var source = Substitute.For<ILogger>();
            var scope = Substitute.For<IDisposable>();

            source.BeginScope(state).Returns(scope);

            var sut = new CacheLogger(source);

            var actual = sut.BeginScope(state);

            actual.Should().NotBeNull();
            actual.Should().NotBeSameAs(scope);
        }

        [Fact]
        public void CacheIsEmptyWhenNoLogEntriesWrittenTest()
        {
            var sut = new CacheLogger();

            sut.Count.Should().Be(0);
            sut.Entries.Should().BeEmpty();
            sut.Last.Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsEnabledReturnsSourceLoggerIsEnabledTest(bool expected)
        {
            var level = LogLevel.Error;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(level).Returns(expected);

            var sut = new CacheLogger(source);

            var actual = sut.IsEnabled(level);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsEnabledReturnsTrueWithNullSourceLoggerTest()
        {
            var level = LogLevel.Error;

            var sut = new CacheLogger();

            var actual = sut.IsEnabled(level);

            actual.Should().BeTrue();
        }

        [Fact]
        public void LogIgnoresNullFormattedMessageTest()
        {
            var exception = new TimeoutException();

            var sut = new CacheLogger();

            var cacheLogger = sut.WithCache();

            cacheLogger.LogInformation(exception, null);

            cacheLogger.Count.Should().Be(1);
            cacheLogger.Last.Exception.Should().Be(exception);
            cacheLogger.Last.Message.Should().BeNull();
        }

        [Fact]
        public void LastReturnsLastLogEntryTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var otherState = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => data;

            var sut = new CacheLogger();

            sut.Log(logLevel, eventId, otherState, exception, formatter);
            sut.Log(logLevel, eventId, state, exception, formatter);

            sut.Count.Should().Be(2);
            sut.Entries.Should().HaveCount(2);
            sut.Last.Message.Should().Be(data);
            sut.Last.Should().Be(sut.Entries.Last());
        }

        [Fact]
        public void LastReturnsLogEntryTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => data;

            var sut = new CacheLogger();

            sut.Log(logLevel, eventId, state, exception, formatter);

            sut.Last.EventId.Should().Be(eventId);
            sut.Last.Exception.Should().Be(exception);
            sut.Last.LogLevel.Should().Be(logLevel);
            sut.Last.State.Should().Be(state);
            sut.Last.Message.Should().Be(data);
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
            Func<string, Exception, string> formatter = (message, error) => data;

            var sut = new CacheLogger();

            sut.Log(logLevel, eventId, state, exception, formatter);

            sut.Entries.Should().HaveCount(1);

            var entry = sut.Entries.Single();

            entry.EventId.Should().Be(eventId);
            entry.Exception.Should().Be(exception);
            entry.LogLevel.Should().Be(logLevel);
            entry.State.Should().Be(state);
            entry.Message.Should().Be(data);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void LogDoesLogsRecordWhenFormatterReturnsEmptyMessageAndExceptionIsNotNullTest(string data)
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => data;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(logLevel).Returns(true);

            var sut = new CacheLogger(source);

            sut.Log(logLevel, eventId, state, exception, formatter);

            source.Received(1).Log(logLevel, eventId, state, exception, formatter);
            sut.Entries.Should().NotBeEmpty();
            sut.Last.Should().NotBeNull();
        }

        [Fact]
        public void LogDoesLogsRecordWhenFormatterReturnsMessageAndExceptionIsNullTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            Func<string, Exception, string> formatter = (message, error) => data;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(logLevel).Returns(true);

            var sut = new CacheLogger(source);

            sut.Log(logLevel, eventId, state, null, formatter);

            source.Received(1).Log(logLevel, eventId, state, null, formatter);
            sut.Entries.Should().NotBeEmpty();
            sut.Last.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void LogDoesNotLogRecordWhenFormatterReturnsEmptyMessageAndExceptionIsNullTest(string data)
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            Func<string, Exception, string> formatter = (message, error) => data;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(logLevel).Returns(true);

            var sut = new CacheLogger(source);

            sut.Log(logLevel, eventId, state, null, formatter);

            source.DidNotReceive().Log(Arg.Any<LogLevel>(), Arg.Any<EventId>(), Arg.Any<string>(), Arg.Any<Exception>(),
                Arg.Any<Func<string, Exception, string>>());
            sut.Entries.Should().BeEmpty();
            sut.Last.Should().BeNull();
        }

        [Fact]
        public void LogDoesNotLogRecordWhenIsEnabledReturnsFalseTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => data;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(logLevel).Returns(false);

            var sut = new CacheLogger(source);

            sut.Log(logLevel, eventId, state, exception, formatter);

            source.DidNotReceive().Log(Arg.Any<LogLevel>(), Arg.Any<EventId>(), Arg.Any<string>(), Arg.Any<Exception>(),
                Arg.Any<Func<string, Exception, string>>());
            sut.Entries.Should().BeEmpty();
            sut.Last.Should().BeNull();
        }

        [Fact]
        public void LogSendsLogToLoggerTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => data;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(logLevel).Returns(true);

            var sut = new CacheLogger(source);

            sut.Log(logLevel, eventId, state, exception, formatter);

            source.Received(1).Log(logLevel, eventId, state, exception, formatter);
        }

        [Fact]
        public void LogThrowsExceptionWithNullFormatterTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var logger = Substitute.For<ILogger>();

            var sut = new CacheLogger(logger);

            Action action = () => sut.Log(logLevel, eventId, state, exception, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullSourceLoggerTest()
        {
            Action action = () => new CacheLogger(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogEntryContainsSnapshotOfActiveScopes()
        {
            var sut = new CacheLogger();

            var state = Guid.NewGuid();

            using (sut.BeginScope(state))
            {
                sut.LogDebug(Guid.NewGuid().ToString());
            }

            sut.Last.Scopes.Single().Should().Be(state);
        }
    }
}