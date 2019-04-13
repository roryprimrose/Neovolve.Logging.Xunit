namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
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
            var scopes = new object[] {Guid.NewGuid().ToString()};

            var sut = new LogEntry(level, eventId, state, exception, message, scopes);

            sut.EventId.Should().Be(eventId);
            sut.Exception.Should().Be(exception);
            sut.State.Should().Be(state);
            sut.LogLevel.Should().Be(level);
            sut.Message.Should().Be(message);
            sut.Scopes.Should().BeEquivalentTo(scopes);
        }
    }
}