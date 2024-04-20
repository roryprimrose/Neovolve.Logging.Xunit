namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;

    public class DefaultScopeFormatterTests
    {
        private readonly ITestOutputHelper _output;

        public DefaultScopeFormatterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void FormatReturnsRawValue()
        {
            var config = new LoggingConfig();
            var scopeLevel = 1;
            var categoryName = Guid.NewGuid().ToString();
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();

            var sut = new DefaultScopeFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message, null);

            _output.WriteLine(actual);

            actual.Should().Be($"   {message}");
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

            var sut = new DefaultScopeFormatter(config);

            var actual = sut.Format(scopeLevel, categoryName, logLevel, eventId, message, exception);

            _output.WriteLine(actual);

            actual.Should().Contain(message);
            actual.Should().Contain(exception.ToString());
            actual.Should().NotContain(logLevel.ToString());
            actual.Should().NotContain(eventId.Id.ToString());
        }
    }
}