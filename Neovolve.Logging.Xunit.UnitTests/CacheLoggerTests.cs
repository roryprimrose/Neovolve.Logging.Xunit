namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
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

            logger.BeginScope(state).Returns((IDisposable)null!);

            using var sut = new CacheLogger(logger);

            using var actual = sut.BeginScope(state);
            actual.Should().NotBeNull();
        }

        [Fact]
        public void BeginScopeReturnsWrappedInstanceWhenSourceLoggerProvided()
        {
            var state = Guid.NewGuid().ToString();

            var source = Substitute.For<ILogger>();
            var scope = Substitute.For<IDisposable>();

            source.BeginScope(state).Returns(scope);

            using var sut = new CacheLogger(source);

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
            // ReSharper disable once DisposeOnUsingVariable
            var action = () => sut.Dispose();

            action.Should().NotThrow();
        }

        [Fact]
        public void ClearEntriesResetsStoredData()
        {
            using var sut = new CacheLogger();

            sut.LogInformation("first");
            sut.LogInformation("second");
            sut.LogInformation("third");
            sut.ClearEntries();

            sut.Count.Should().Be(0);
            sut.Entries.Should().BeEmpty();
            sut.Last.Should().BeNull();
        }

        [Fact]
        public void DisposeCallsDisposeOnFactory()
        {
            var logger = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            using var sut = new CacheLogger(logger, factory);

            // ReSharper disable once DisposeOnUsingVariable
            sut.Dispose();

            factory.Received().Dispose();
        }

        [Fact]
        public void EntriesRecordsLogStateInformation()
        {
            using var sut = new CacheLogger();

            var first = Guid.NewGuid();
            var second = Guid.NewGuid().ToString();
            var third = Environment.TickCount;
            var format = "This {First} value is not the same as {Second}. We are also interested in {Third}.";

            sut.LogInformation(format,
                first, second, third);

            var entry = sut.Last!;

            entry.State.Should().NotBeNull();

            var data = ((IEnumerable<KeyValuePair<string, object>>)entry.State).ToList();

            data.Should().NotBeNull();
            data.Should().ContainKey("First").WhoseValue.Should().Be(first);
            data.Should().ContainKey("Second").WhoseValue.Should().Be(second);
            data.Should().ContainKey("Third").WhoseValue.Should().Be(third);
            data.Should().ContainKey("{OriginalFormat}").WhoseValue.Should().Be(format);
        }

        [Fact]
        public void EntriesReturnsRecordsEntriesAfterBeingCleared()
        {
            using var sut = new CacheLogger();

            sut.LogInformation("first");
            sut.LogInformation("second");
            sut.LogInformation("third");
            sut.ClearEntries();
            sut.LogInformation("first");
            sut.LogInformation("second");
            sut.LogInformation("third");

            var entries = sut.Entries.ToList();

            entries.Should().HaveCount(3);
            entries[0].Message.Should().Be("first");
            entries[1].Message.Should().Be("second");
            entries[2].Message.Should().Be("third");
            sut.Last!.Message.Should().Be("third");
        }

        [Fact]
        public void EntriesReturnsRecordsEntriesInOrder()
        {
            using var sut = new CacheLogger();

            sut.LogInformation("first");
            sut.LogInformation("second");
            sut.LogInformation("third");

            var entries = sut.Entries.ToList();

            entries.Should().HaveCount(3);
            entries[0].Message.Should().Be("first");
            entries[1].Message.Should().Be("second");
            entries[2].Message.Should().Be("third");
            sut.Last!.Message.Should().Be("third");
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsEnabledReturnsSourceLoggerIsEnabled(bool expected)
        {
            const LogLevel level = LogLevel.Error;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(level).Returns(expected);

            using var sut = new CacheLogger(source);

            var actual = sut.IsEnabled(level);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsEnabledReturnsTrueWithNullSourceLogger()
        {
            const LogLevel level = LogLevel.Error;

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

            sut.Last!.Scopes.Should().HaveCount(1);
            sut.Last.Scopes.Single().Should().Be(state);
        }

        [Fact]
        public void LastLogEntryCapturesScopeStateWhenSourceLoggerBeginScopeReturnsNull()
        {
            var state = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();

            var logger = Substitute.For<ILogger>();

            logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);
            logger.BeginScope(state).Returns((IDisposable)null!);

            using var sut = new CacheLogger(logger);

            using (sut.BeginScope(state))
            {
                sut.LogInformation(message);
            }

            sut.Last!.Scopes.Should().HaveCount(1);
            sut.Last.Scopes.Single().Should().Be(state);
        }

        [Fact]
        public void LastReturnsLastLogEntry()
        {
            const LogLevel logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var otherState = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception? error) => data;

            using var sut = new CacheLogger();

            sut.Log(logLevel, eventId, otherState, exception, Formatter);
            sut.Log(logLevel, eventId, state, exception, Formatter);

            sut.Count.Should().Be(2);
            sut.Entries.Should().HaveCount(2);
            sut.Last!.Message.Should().Be(data);
            sut.Last.Should().Be(sut.Entries.Last());
        }

        [Fact]
        public void LastReturnsLogEntry()
        {
            const LogLevel logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception? error) => data;

            using var sut = new CacheLogger();

            sut.Log(logLevel, eventId, state, exception, Formatter);

            sut.Last!.EventId.Should().Be(eventId);
            sut.Last.Exception.Should().Be(exception);
            sut.Last.LogLevel.Should().Be(logLevel);
            sut.Last.State.Should().ContainKey("State").WhoseValue.Should().Be(state);
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
        public void LogCachesLogMessage(LogLevel logLevel)
        {
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception? error) => data;

            using var sut = new CacheLogger();

            sut.Log(logLevel, eventId, state, exception, Formatter);

            sut.Entries.Should().HaveCount(1);

            var entry = sut.Entries.Single();

            entry.EventId.Should().Be(eventId);
            entry.Exception.Should().Be(exception);
            entry.LogLevel.Should().Be(logLevel);
            sut.Last!.State.Should().ContainKey("State").WhoseValue.Should().Be(state);
            entry.Message.Should().Be(data);
        }

        [Fact]
        public async Task LogCanCacheLogMessagesFromMultipleThreads()
        {
            var cancellationToken = TestContext.Current.CancellationToken;
            const int count = 1000;
            var tasks = new List<Task>(count);
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception? error) => data;

            using var sut = new CacheLogger();

            for (var index = 0; index < count; index++)
            {
                var task = Task.Run(async () =>
                {
                    await Task.Delay(100, cancellationToken);

                    // ReSharper disable once AccessToDisposedClosure
                    sut.Log(LogLevel.Error, eventId, state, exception, Formatter);
                }, cancellationToken);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            sut.Entries.Should().HaveCount(count);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void LogDoesLogsRecordWhenFormatterReturnsEmptyMessageAndExceptionIsNotNull(string? data)
        {
            const LogLevel logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception? error) => data!;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(logLevel).Returns(true);

            using var sut = new CacheLogger(source);

            sut.Log(logLevel, eventId, state, exception, Formatter);

            source.Received(1).Log(logLevel, eventId, state, exception, Formatter);
            sut.Entries.Should().NotBeEmpty();
            sut.Last.Should().NotBeNull();
        }

        [Fact]
        public void LogDoesLogsRecordWhenFormatterReturnsMessageAndExceptionIsNull()
        {
            const LogLevel logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            string Formatter(string message, Exception? error) => data;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(logLevel).Returns(true);

            using var sut = new CacheLogger(source);

            sut.Log(logLevel, eventId, state, null, Formatter);

            source.Received(1).Log(logLevel, eventId, state, null, Formatter);
            sut.Entries.Should().NotBeEmpty();
            sut.Last.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void LogDoesNotLogRecordWhenFormatterReturnsEmptyMessageAndExceptionIsNull(string? data)
        {
            const LogLevel logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            string Formatter(string message, Exception? error) => data!;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(logLevel).Returns(true);

            using var sut = new CacheLogger(source);

            sut.Log(logLevel, eventId, state, null, Formatter);

            source.DidNotReceive()
                .Log(Arg.Any<LogLevel>(),
                    Arg.Any<EventId>(),
                    Arg.Any<string>(),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<string, Exception?, string>>());
            sut.Entries.Should().BeEmpty();
            sut.Last.Should().BeNull();
        }

        [Fact]
        public void LogDoesNotLogRecordWhenIsEnabledReturnsFalse()
        {
            const LogLevel logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception? error) => data;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(logLevel).Returns(false);

            using var sut = new CacheLogger(source);

            sut.Log(logLevel, eventId, state, exception, Formatter);

            source.DidNotReceive()
                .Log(Arg.Any<LogLevel>(),
                    Arg.Any<EventId>(),
                    Arg.Any<string>(),
                    Arg.Any<Exception>(),
                    Arg.Any<Func<string, Exception?, string>>());
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

            sut.Last!.Scopes.Single().Should().Be(state);
        }

        [Fact]
        public void LogIgnoresLogWrittenEventWhenNotReferenced()
        {
            using var sut = new CacheLogger();

            // ReSharper disable once AccessToDisposedClosure
            var action = () => sut.Log(LogLevel.Information, new EventId(), "Test", null,
                (state, _) => state.ToString());

            action.Should().NotThrow();
        }

        [Fact]
        public void LogIgnoresNullFormattedMessage()
        {
            var exception = new TimeoutException();

            using var sut = new CacheLogger();

            sut.LogInformation(exception, null);

            sut.Count.Should().Be(1);
            sut.Last!.Exception.Should().Be(exception);
            sut.Last.Message.Should().BeEmpty();
        }

        [Fact]
        public void LogRaisesLogWrittenEvent()
        {
            using var sut = new CacheLogger();

            var eventRaised = false;

            sut.LogWritten += (source, logEvent) =>
            {
                // ReSharper disable once AccessToDisposedClosure
                source.Should().Be(sut);
                logEvent.Should().NotBeNull();

                eventRaised = true;
            };

            sut.Log(LogLevel.Information, new EventId(), "Test", null, (state, _) => state.ToString());

            eventRaised.Should().BeTrue();
        }

        [Fact]
        public void LogSendsLogToLogger()
        {
            const LogLevel logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string message, Exception? error) => data;

            var source = Substitute.For<ILogger>();

            source.IsEnabled(logLevel).Returns(true);

            using var sut = new CacheLogger(source);

            sut.Log(logLevel, eventId, state, exception, Formatter);

            source.Received(1).Log(logLevel, eventId, state, exception, Formatter);
        }

        [Fact]
        public void LogThrowsExceptionWithNullFormatter()
        {
            const LogLevel logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var logger = Substitute.For<ILogger>();

            using var sut = new CacheLogger(logger);

            // ReSharper disable once AccessToDisposedClosure
            var action = () => sut.Log(logLevel, eventId, state, exception, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullSourceLogger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new CacheLogger(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}