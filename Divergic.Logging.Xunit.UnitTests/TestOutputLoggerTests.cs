namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using Divergic.Logging.Xunit;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using NSubstitute;

    public class TestOutputLoggerTests
    {
        [Fact]
        public void BeginScopeReturnsInstanceTest()
        {
            var name = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(name, output);

            var actual = sut.BeginScope(state);

            actual.Should().NotBeNull();

            Action action = () => actual.Dispose();

            action.Should().NotThrow();
        }

        [Theory]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Warning)]
        public void IsEnabledReturnsTrueTest(LogLevel logLevel)
        {
            var state = Guid.NewGuid().ToString();
            var name = Guid.NewGuid().ToString();

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(name, output);

            var actual = sut.IsEnabled(logLevel);

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Warning)]
        public void LogTest(LogLevel logLevel)
        {
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => { return data; };
            var name = Guid.NewGuid().ToString();

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(name, output);

            sut.Log(logLevel, eventId, state, exception, formatter);

            output.Received().WriteLine("{1} [{2}]: {3}", name, logLevel, eventId.Id, data);
            output.Received().WriteLine("{1} [{2}]: {3}", name, logLevel, eventId.Id, exception);
        }
    }
}