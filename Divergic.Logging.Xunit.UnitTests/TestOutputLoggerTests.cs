namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;
    using NSubstitute;

    public class TestOutputLoggerTests
    {
        private readonly ITestOutputHelper _output;

        public TestOutputLoggerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BeginScopeReturnsInstanceTest()
        {
            var name = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();

            var sut = new TestOutputLogger(name, _output);

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
            var name = Guid.NewGuid().ToString();

            var sut = new TestOutputLogger(name, _output);

            var actual = sut.IsEnabled(logLevel);

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void LogDoesNotWriteEmptyMessageTest(string message)
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            Func<string, Exception, string> formatter = (logState, error) => { return message; };
            var name = Guid.NewGuid().ToString();

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(name, output);

            sut.Log(logLevel, eventId, state, null, formatter);

            output.DidNotReceive().WriteLine(Arg.Any<string>(), Arg.Any<object[]>());
        }

        [Fact]
        public void LogDoesNotWriteNullExceptionTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            Func<string, Exception, string> formatter = (logState, error) => { return data; };
            var name = Guid.NewGuid().ToString();

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(name, output);

            sut.Log(logLevel, eventId, state, null, formatter);

            output.Received(1).WriteLine(Arg.Any<string>(), Arg.Any<object[]>());
            output.Received().WriteLine("{0}{2} [{3}]: {4}", string.Empty, name, logLevel, eventId.Id, data);
        }

        [Fact]
        public void LogIgnoresNullFormattedMessageTest()
        {
            var name = Guid.NewGuid().ToString();
            var exception = new TimeoutException();

            var sut = new TestOutputLogger(name, _output);

            var cacheLogger = sut.WithCache();

            cacheLogger.LogInformation(exception, null);

            cacheLogger.Count.Should().Be(1);
            cacheLogger.Last.Exception.Should().Be(exception);
            cacheLogger.Last.Message.Should().BeNull();
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

            output.Received().WriteLine("{0}{2} [{3}]: {4}", string.Empty, name, logLevel, eventId.Id, data);
            output.Received().WriteLine("{0}{2} [{3}]: {4}", string.Empty, name, logLevel, eventId.Id, exception);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ThrowsExceptionWhenCreatedWithInvalidNameTest(string name)
        {
            var output = Substitute.For<ITestOutputHelper>();

            Action action = () => new TestOutputLogger(name, null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOutputTest()
        {
            var name = Guid.NewGuid().ToString();

            Action action = () => new TestOutputLogger(name, null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}