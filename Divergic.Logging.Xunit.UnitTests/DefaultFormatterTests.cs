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
        [InlineData(null, "this message", "this message")]
        [InlineData("secret", "this message", "this message")]
        [InlineData("secret", "this secret message", "this **** message")]
        public void FormatHidesSensitiveDataInException(string? sensitiveValue, string message, string expected)
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var logMessage = Guid.NewGuid().ToString();
            var exception = new InvalidOperationException(message);

            if (sensitiveValue != null)
            {
                config.SensitiveValues.Add(sensitiveValue);
            }

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, logMessage, exception);

            actual.Should().Contain(expected);

            if (sensitiveValue != null)
            {
                actual.Should().NotContain(sensitiveValue);
            }
        }

        [Theory]
        [InlineData(null, "this message", "this message")]
        [InlineData("secret", "this secret message", "this **** message")]
        public void FormatHidesSensitiveDataInMessage(string? sensitiveValue, string message, string expected)
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();

            if (sensitiveValue != null)
            {
                config.SensitiveValues.Add(sensitiveValue);
            }

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message, null);

            actual.Should().Be($"   Information [{eventId.Id}]: {expected}");
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("  ", false)]
        [InlineData("stuff", false)]
        [InlineData(null, true)]
        [InlineData("", true)]
        [InlineData("  ", true)]
        [InlineData("stuff", true)]
        public void FormatIncludesNewLineBetweenMessageAndException(string? message, bool exceptionExists)
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            Exception? exception = exceptionExists
                ? new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
                : null;

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message!, exception);

            actual.Should().NotStartWith(Environment.NewLine);
            actual.Should().NotEndWith(Environment.NewLine);

            if (string.IsNullOrWhiteSpace(message))
            {
                if (exception != null)
                {
                    actual.Should().Be($"   Information [{eventId.Id}]: {exception}");
                }
                else
                {
                    actual.Should().BeEmpty();
                }
            }
            else if (exception != null)
            {
                actual.Should()
                    .Be(
                        $"   Information [{eventId.Id}]: stuff{Environment.NewLine}   Information [{eventId.Id}]: {exception}");
            }
            else
            {
                actual.Should().Be($"   Information [{eventId.Id}]: stuff");
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void FormatReturnsEmptyWhenMessageIsNullEmptyOrWhiteSpace(string? message)
        {
            var config = new LoggingConfig().Set(x => x.ScopePaddingSpaces = 2);
            var scopeLevel = 1;
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message!, null);

            actual.Should().BeEmpty();
        }

        [Fact]
        public void FormatReturnsValueWithEventId()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message, exception);

            _output.WriteLine(actual);

            actual.Should().Contain(eventId.Id.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void FormatReturnsValueWithException()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message, exception);

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
            var categoryName = Guid.NewGuid().ToString();

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message, null);

            _output.WriteLine(actual);

            actual.Should().Contain(logLevel.ToString());
        }

        [Fact]
        public void FormatReturnsValueWithMessage()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message, exception);

            _output.WriteLine(actual);

            actual.Should().Contain(message);
        }

        [Fact]
        public void FormatReturnsValueWithoutException()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message, null);

            _output.WriteLine(actual);

            actual.Should().NotContain("Exception");
        }

        [Fact]
        public void FormatReturnsValueWithoutName()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message, exception);

            _output.WriteLine(actual);

            actual.Should().NotContain(categoryName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        public void FormatReturnsValueWithPadding(int scopeLevel)
        {
            var config = new LoggingConfig();
            var padding = new string(' ', config.ScopePaddingSpaces * scopeLevel);
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            var sut = new DefaultFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message, exception);

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
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new DefaultFormatter(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}