namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using global::Xunit;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;

    public class LogEntryTests
    {
        [Fact]
        public void ReturnsConstructorValuesInProperties()
        {
            var level = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var message = Guid.NewGuid().ToString();
            var scopes = new object[] { Guid.NewGuid().ToString() };

            var sut = new LogEntry(level, eventId, state, exception, message, scopes);

            sut.EventId.Should().Be(eventId);
            sut.Exception.Should().Be(exception);
            sut.LogLevel.Should().Be(level);
            sut.Message.Should().Be(message);
            sut.Scopes.Should().BeEquivalentTo(scopes);
        }

        [Fact]
        public void StateReturnsEmptyWhenStateValueIsNull()
        {
            var level = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            object? state = null;
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var message = Guid.NewGuid().ToString();
            var scopes = new object[] { Guid.NewGuid().ToString() };

            var sut = new LogEntry(level, eventId, state, exception, message, scopes);

            sut.State.Should().BeEmpty();
        }

        [Fact]
        public void StateReturnsStateAsEntryWhenNotExpectedType()
        {
            var level = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            object? state = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var message = Guid.NewGuid().ToString();
            var scopes = new object[] { Guid.NewGuid().ToString() };

            var sut = new LogEntry(level, eventId, state, exception, message, scopes);

            sut.State.Should().ContainKey("State").WhoseValue.Should().Be(state);
        }

        [Fact]
        public void StateReturnsStateAsStrongTypeWhenExpectedType()
        {
            var level = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var first = Guid.NewGuid();
            var second = Guid.NewGuid().ToString();
            var third = Environment.TickCount;
            IReadOnlyList<KeyValuePair<string, object?>> state = new List<KeyValuePair<string, object?>>
            {
                new("First", first),
                new("Second", second),
                new("Third", third)
            };
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var message = Guid.NewGuid().ToString();
            var scopes = new object[] { Guid.NewGuid().ToString() };

            var sut = new LogEntry(level, eventId, state, exception, message, scopes);

            sut.State.Should().ContainKey("First").WhoseValue.Should().Be(first);
            sut.State.Should().ContainKey("Second").WhoseValue.Should().Be(second);
            sut.State.Should().ContainKey("Third").WhoseValue.Should().Be(third);
        }
    }
}