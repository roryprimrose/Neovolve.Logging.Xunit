namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using System.Globalization;
    using System.Text;
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

        [Fact]
        public void BeginScopeShouldNotThrowWhenStateDataHasCircularReference()
        {
            var name = Guid.NewGuid().ToString();
            var state = new CircularReference();

            var sut = new TestOutputLogger(name, _output);

            using (sut.BeginScope(state))
            {
                sut.LogDebug("...");
            }
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

            Action action = () => new TestOutputLogger(name, output);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOutputTest()
        {
            var name = Guid.NewGuid().ToString();

            Action action = () => new TestOutputLogger(name, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LogCustomFormatter()
        {
            var output = Substitute.For<ITestOutputHelper>();

            var loggerFactory = new LoggerFactory().AddXunit(output, MyCustomFormatter);
            //var loggerFactory = LogFactory.Create(output, MyCustomFormatter);

            var logger = loggerFactory.CreateLogger("category");
            logger.Log(LogLevel.Debug, "message");

            output.Received().WriteLine("Debug category message");
        }

        // This an example message formatter.
        public static string MyCustomFormatter(int scopeLevel, string name, LogLevel logLevel, EventId eventId, string message, Exception exception)
        {
            var sb = new StringBuilder();

            if (scopeLevel > 0)
                sb.Append(' ', scopeLevel * 2);

            sb.Append($"{GetShortLogLevelString(logLevel)} ");

            if (!string.IsNullOrEmpty(name))
                sb.Append($"{name} ");

            if (eventId != null && eventId.Id != 0)
                sb.Append($"[{eventId.Id}]: ");

            if (!string.IsNullOrEmpty(message))
                sb.Append(message);

            if (exception != null)
                sb.Append($"\n{exception}");

            return sb.ToString();
        }

        private static string GetShortLogLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:       return "Trace";
                case LogLevel.Debug:       return "Debug";
                case LogLevel.Information: return "Info ";
                case LogLevel.Warning:     return "Warn ";
                case LogLevel.Error:       return "Error";
                case LogLevel.Critical:    return "Crit ";
                case LogLevel.None:        return "None ";
                default: throw new Exception("invalid");
            }
        }
    }
}