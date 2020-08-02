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
        public void BeginScopeReturnsInstanceWhenNoSourceLoggerProvided()
        {
            var state = Guid.NewGuid().ToString();

            using var sut = new CacheLogger();

            using var actual = sut.BeginScope(state);
            actual.Should().NotBeNull();
        }

        [Fact]
        public void BeginScopeReturnsInstanceWhenSourceLoggerBeginScopeReturnsNull()
        {
            var state = Guid.NewGuid().ToString();

            var logger = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            logger.BeginScope(state).Returns((IDisposable) null);

            using var sut = new CacheLogger(logger, factory);

            using var actual = sut.BeginScope(state);
            actual.Should().NotBeNull();
        }

        [Fact]
        public void BeginScopeReturnsWrappedInstanceWhenSourceLoggerProvided()
        {
            var state = Guid.NewGuid().ToString();

            var source = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();
            var scope = Substitute.For<IDisposable>();

            source.BeginScope(state).Returns(scope);

            using var sut = new CacheLogger(source, factory);

            var actual = sut.BeginScope(state);

            actual.Should().NotBeNull();
            actual.Should().NotBeSameAs(scope);
        }

        [Fact]
        public void CacheIsEmptyWhenNoLogEntriesWritten()
        {
            using var sut = new CacheLogger();

            sut.Count.Should().Be(0);
            sut.Entries.Should().BeEmpty();
            sut.Last.Should().BeNull();
        }

        [Fact]
        public void CanDisposeWithoutFactory()
        {
            using var sut = new CacheLogger();

            // ReSharper disable once AccessToDisposedClosure
            Action action = () => sut.Dispose();

            action.Should().NotThrow();
        }

        [Fact]
        public void DisposeCallsDisposeOnFactory()
        {
            var logger = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            using var sut = new CacheLogger(logger, factory);

            sut.Dispose();

            factory.Received().Dispose();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsEnabledReturnsSourceLoggerIsEnabledTest(bool expected)
        {
            var level = LogLevel.Error;

            var source = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            source.IsEnabled(level).Returns(expected);

            using var sut = new CacheLogger(source, factory);

            var actual = sut.IsEnabled(level);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsEnabledReturnsTrueWithNullSourceLogger()
        {
            var level = LogLevel.Error;

            using var sut = new CacheLogger();

            var actual = sut.IsEnabled(level);

            actual.Should().BeTrue();
        }

        [Fact]
        public void LastLogEntryCapturesScopeStateWhenNoLoggerProvided()
        {
            var state = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();

            using var sut = new CacheLogger();

            using (sut.BeginScope(state))
            {
                sut.LogInformation(message);
            }

            sut.Last.Scopes.Should().HaveCount(1);
            sut.Last.Scopes.Single().Should().Be(state);
        }

        [Fact]
        public void LastLogEntryCapturesScopeStateWhenSourceLoggerBeginScopeReturnsNull()
        {
            var state = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();

            var factory = Substitute.For<ILoggerFactory>();
            var logger = Substitute.For<ILogger>();

            logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            logger.BeginScope(state).Returns((IDisposable) null);

            using var sut = new CacheLogger(logger, factory);

            using (sut.BeginScope(state))
            {
                sut.LogInformation(message);
            }

            sut.Last.Scopes.Should().HaveCount(1);
            sut.Last.Scopes.Single().Should().Be(state);
        }

        [Fact]
        public void LastReturnsLastLogEntry()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var otherState = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception error) => data;

            using var sut = new CacheLogger();

            sut.Log(logLevel, eventId, otherState, exception, Formatter);
            sut.Log(logLevel, eventId, state, exception, Formatter);

            sut.Count.Should().Be(2);
            sut.Entries.Should().HaveCount(2);
            sut.Last.Message.Should().Be(data);
            sut.Last.Should().Be(sut.Entries.Last());
        }

        [Fact]
        public void LastReturnsLogEntry()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception error) => data;

            using var sut = new CacheLogger();

            sut.Log(logLevel, eventId, state, exception, Formatter);

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
            string Formatter(string message, Exception error) => data;

            using var sut = new CacheLogger();

            sut.Log(logLevel, eventId, state, exception, Formatter);

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
            string Formatter(string message, Exception error) => data;

            var source = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            source.IsEnabled(logLevel).Returns(true);

            using var sut = new CacheLogger(source, factory);

            sut.Log(logLevel, eventId, state, exception, Formatter);

            source.Received(1).Log(logLevel, eventId, state, exception, Formatter);
            sut.Entries.Should().NotBeEmpty();
            sut.Last.Should().NotBeNull();
        }

        [Fact]
        public void LogDoesLogsRecordWhenFormatterReturnsMessageAndExceptionIsNull()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            string Formatter(string message, Exception error) => data;

            var source = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            source.IsEnabled(logLevel).Returns(true);

            using var sut = new CacheLogger(source, factory);

            sut.Log(logLevel, eventId, state, null, Formatter);

            source.Received(1).Log(logLevel, eventId, state, null, Formatter);
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
            string Formatter(string message, Exception error) => data;

            var source = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            source.IsEnabled(logLevel).Returns(true);

            using var sut = new CacheLogger(source, factory);

            sut.Log(logLevel, eventId, state, null, Formatter);

            source.DidNotReceive()
                .Log(Arg.Any<LogLevel>(),
                    Arg.Any<EventId>(),
                    Arg.Any<string>(),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<string, Exception, string>>());
            sut.Entries.Should().BeEmpty();
            sut.Last.Should().BeNull();
        }

        [Fact]
        public void LogDoesNotLogRecordWhenIsEnabledReturnsFalse()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception error) => data;

            var source = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            source.IsEnabled(logLevel).Returns(false);

            using var sut = new CacheLogger(source, factory);

            sut.Log(logLevel, eventId, state, exception, Formatter);

            source.DidNotReceive()
                .Log(Arg.Any<LogLevel>(),
                    Arg.Any<EventId>(),
                    Arg.Any<string>(),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<string, Exception, string>>());
            sut.Entries.Should().BeEmpty();
            sut.Last.Should().BeNull();
        }

        [Fact]
        public void LogEntriesIdentifyAllRecordedScopes()
        {
            var firstScopeState = Guid.NewGuid().ToString();
            var secondScopeState = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();

            using var sut = new CacheLogger();

            sut.LogInformation("Before any scopes");

            using (sut.BeginScope(firstScopeState))
            {
                sut.LogInformation(message);

                using (sut.BeginScope(secondScopeState))
                {
                    sut.LogInformation(message);
                }
            }

            var entries = sut.Entries.ToList();

            entries.Should().HaveCount(3);
            entries[0].Scopes.Should().BeEmpty();
            entries[1].Scopes.Should().HaveCount(1);
            entries[1].Scopes.First().Should().Be(firstScopeState);
            entries[2].Scopes.Should().HaveCount(2);
            entries[2].Scopes.First().Should().Be(secondScopeState);
            entries[2].Scopes.Skip(1).First().Should().Be(firstScopeState);
        }

        [Fact]
        public void LogEntryContainsSnapshotOfActiveScopes()
        {
            using var sut = new CacheLogger();

            var state = Guid.NewGuid();

            using (sut.BeginScope(state))
            {
                sut.LogDebug(Guid.NewGuid().ToString());
            }

            sut.Last.Scopes.Single().Should().Be(state);
        }

        [Fact]
        public void LogIgnoresNullFormattedMessage()
        {
            var exception = new TimeoutException();

            var factory = Substitute.For<ILoggerFactory>();

            using var sut = new CacheLogger();

            using var cacheLogger = sut.WithCache(factory);

            cacheLogger.LogInformation(exception, null);

            cacheLogger.Count.Should().Be(1);
            cacheLogger.Last.Exception.Should().Be(exception);
            cacheLogger.Last.Message.Should().BeNull();
        }

        [Fact]
        public void LogSendsLogToLogger()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception error) => data;

            var source = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            source.IsEnabled(logLevel).Returns(true);

            using var sut = new CacheLogger(source, factory);

            sut.Log(logLevel, eventId, state, exception, Formatter);

            source.Received(1).Log(logLevel, eventId, state, exception, Formatter);
        }

        [Fact]
        public void LogThrowsExceptionWithNullFormatter()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var logger = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            using var sut = new CacheLogger(logger, factory);

            // ReSharper disable once AccessToDisposedClosure
            Action action = () => sut.Log(logLevel, eventId, state, exception, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullFactory()
        {
            var logger = Substitute.For<ILogger>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new CacheLogger(logger, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullSourceLogger()
        {
            var factory = Substitute.For<ILoggerFactory>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new CacheLogger(null, factory);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}