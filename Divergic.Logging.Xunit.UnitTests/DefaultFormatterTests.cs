namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;

    public class DefaultFormatterTests
    {
        private readonly ITestOutputHelper _output;

        public DefaultFormatterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void FormatReturnsEmptyWhenMessageIsNullEmptyOrWhiteSpace(string message)
        {
            var config = new LoggingConfig().Set(x => x.ScopePaddingSpaces = 2);
            var scopeLevel = 1;
            var name = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, name, logLevel, eventId, message, null);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void FormatReturnsValueWithEventId()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var name = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, name, logLevel, eventId, message, exception);

            _output.WriteLine(actual);

            actual.Should().Contain(eventId.Id.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void FormatReturnsValueWithException()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var name = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, name, logLevel, eventId, message, exception);

            _output.WriteLine(actual);

            actual.Should().Contain(exception.ToString());
        }

        [Theory]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Warning)]
        public void FormatReturnsValueWithLogLevel(LogLevel logLevel)
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, name, logLevel, eventId, message, null);

            _output.WriteLine(actual);

            actual.Should().Contain(logLevel.ToString());
        }

        [Fact]
        public void FormatReturnsValueWithMessage()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var name = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, name, logLevel, eventId, message, exception);

            _output.WriteLine(actual);

            actual.Should().Contain(message);
        }

        [Fact]
        public void FormatReturnsValueWithoutException()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var name = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, name, logLevel, eventId, message, null);

            _output.WriteLine(actual);

            actual.Should().NotContain("Exception");
        }

        [Fact]
        public void FormatReturnsValueWithoutName()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var name = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, name, logLevel, eventId, message, exception);

            _output.WriteLine(actual);

            actual.Should().NotContain(name);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        public void FormatReturnsValueWithPadding(int scopeLevel)
        {
            var config = new LoggingConfig();
            var padding = new string(' ', config.ScopePaddingSpaces * scopeLevel);
            var name = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, name, logLevel, eventId, message, exception);

            _output.WriteLine(actual);

            if (scopeLevel > 0)
            {
                actual.Should().StartWith(padding);
            }
            else
            {
                actual.Should().NotStartWith(" ");
            }
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullConfig()
        {
            Action action = () => new DefaultFormatter(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}