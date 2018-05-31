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
        public void ReturnsConstructorValuesInPropertiesTest()
        {
            var level = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var message = Guid.NewGuid().ToString();

            var sut = new LogEntry(level, eventId, state, exception, message);

            sut.EventId.Should().Be(eventId);
            sut.Exception.Should().Be(exception);
            sut.State.Should().Be(state);
            sut.LogLevel.Should().Be(level);
            sut.Message.Should().Be(message);
        }
    }
}