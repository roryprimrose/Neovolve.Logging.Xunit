namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
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
        public void BeginScopeReturnsInstance()
        {
            var categoryName = Guid.NewGuid().ToString();
            var state = Guid.NewGuid().ToString();

            var sut = new TestOutputLogger(categoryName, _output);

            var actual = sut.BeginScope(state);

            actual.Should().NotBeNull();

            var action = () => actual.Dispose();

            action.Should().NotThrow();
        }

        [Fact]
        public void BeginScopeShouldNotThrowWhenStateDataHasCircularReference()
        {
            var categoryName = Guid.NewGuid().ToString();
            var state = new CircularReference();

            var sut = new TestOutputLogger(categoryName, _output);

            using (sut.BeginScope(state))
            {
                sut.LogDebug("...");
            }
        }

        [Theory]
        [ClassData(typeof(LogLevelDataSet))]
        public void IsEnabledReturnsBasedOnConfiguredLevel(LogLevel configuredLevel, LogLevel logLevel,
            bool isEnabled)
        {
            var categoryName = Guid.NewGuid().ToString();
            var config = new LoggingConfig
            {
                LogLevel = configuredLevel
            };

            var sut = new TestOutputLogger(categoryName, _output, config);

            var actual = sut.IsEnabled(logLevel);

            actual.Should().Be(isEnabled);
        }

        [Fact]
        public void LogIgnoresTestBoundaryFailure()
        {
            // This test should not fail the test runner
            var categoryName = Guid.NewGuid().ToString();
            var config = new LoggingConfig { IgnoreTestBoundaryException = true };

            var sut = new TestOutputLogger(categoryName, _output, config);

            var task = new Task(async () =>
            {
                await Task.Delay(0);

                sut.LogCritical("message2");
            });

            task.Start();
        }

        [Fact]
        public void LogUsesDefaultFormatterWhenConfigIsNull()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();
            string Formatter(string logState, Exception? error) => message;
            var categoryName = Guid.NewGuid().ToString();
            var expected = string.Format(CultureInfo.InvariantCulture,
                "{0}{1} [{2}]: {3}",
                string.Empty,
                logLevel,
                eventId.Id,
                message);

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(categoryName, output);

            sut.Log(logLevel, eventId, state, null, Formatter);

            output.Received(1).WriteLine(Arg.Any<string>());
            output.Received().WriteLine(expected);
        }

        [Fact]
        public void LogWritesException()
        {
            var logLevel = LogLevel.Information;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            string Formatter(string logState, Exception? error) => message;
            var categoryName = Guid.NewGuid().ToString();

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(categoryName, output);

            sut.Log(logLevel, eventId, state, exception, Formatter);

            output.Received(1).WriteLine(Arg.Any<string>());
            output.Received()
                .WriteLine(Arg.Is<string>(x => x.Contains(exception.ToString(), StringComparison.OrdinalIgnoreCase)));
        }

        [Theory]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Warning)]
        public void LogWritesLogLevelToOutput(LogLevel logLevel)
        {
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();
            string Formatter(string logState, Exception? error) => message;
            var categoryName = Guid.NewGuid().ToString();
            var expected = string.Format(CultureInfo.InvariantCulture,
                "{0}{1} [{2}]: {3}",
                string.Empty,
                logLevel,
                eventId.Id,
                message);

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(categoryName, output);

            sut.Log(logLevel, eventId, state, null, Formatter);

            output.Received(1).WriteLine(Arg.Any<string>());
            output.Received().WriteLine(expected);
        }

        [Fact]
        public void LogWritesMessageUsingSpecifiedLineFormatter()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            var categoryName = Guid.NewGuid().ToString();
            var expected = Guid.NewGuid().ToString();
            string Formatter(string logState, Exception? error) => message;

            var formatter = Substitute.For<ILogFormatter>();
            var config = new LoggingConfig { Formatter = formatter };

            formatter.Format(0, categoryName, logLevel, eventId, message, exception).Returns(expected);

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(categoryName, output, config);

            sut.Log(logLevel, eventId, state, exception, Formatter);

            formatter.Received().Format(0, categoryName, logLevel, eventId, message, exception);

            output.Received().WriteLine(expected);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ThrowsExceptionWhenCreatedWithInvalidCategoryName(string? categoryName)
        {
            var output = Substitute.For<ITestOutputHelper>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new TestOutputLogger(categoryName!, output);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOutput()
        {
            var categoryName = Guid.NewGuid().ToString();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new TestOutputLogger(categoryName, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WriteLogIgnoresExceptionWhenIgnoreTestBoundaryExceptionEnabled()
        {
            var categoryName = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();
            var config = new LoggingConfig
            {
                IgnoreTestBoundaryException = true
            };

            var output = Substitute.For<ITestOutputHelper>();

            output.When(x => x.WriteLine(Arg.Any<string>())).Throw<InvalidOperationException>();

            var sut = new TestOutputLogger(categoryName, output, config);

            var action = () => sut.LogInformation(message);

            action.Should().NotThrow();
        }

        [Fact]
        public void WriteLogThrowsExceptionWhenIgnoreTestBoundaryExceptionDisabled()
        {
            var categoryName = Guid.NewGuid().ToString();
            var message = Guid.NewGuid().ToString();
            var config = new LoggingConfig
            {
                IgnoreTestBoundaryException = false
            };

            var output = Substitute.For<ITestOutputHelper>();

            output.When(x => x.WriteLine(Arg.Any<string>())).Throw<InvalidOperationException>();

            var sut = new TestOutputLogger(categoryName, output, config);

            var action = () => sut.LogInformation(message);

            action.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData(null, "this message", "this message")]
        [InlineData("secret", "this message", "this message")]
        [InlineData("secret", "this secret message", "this **** message")]
        public void WritesScopeMessagesUsingFormatter(string? sensitiveValue, string state, string expected)
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var categoryName = Guid.NewGuid().ToString();
            string Formatter(string logState, Exception? error) => message;

            var config = new LoggingConfig();

            if (sensitiveValue != null)
            {
                config.SensitiveValues.Add(sensitiveValue);
            }

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(categoryName, output, config);

            using (sut.BeginScope(state))
            {
                sut.Log(logLevel, eventId, state, null, Formatter);
            }

            output.Received().WriteLine($"<Scope: {expected}>");
            output.Received().WriteLine($"   {logLevel} [{eventId.Id}]: {message}");
            output.Received().WriteLine($"</Scope: {expected}>");
        }

        [Fact]
        public void WritesScopeMessagesWithStructuredDataUsingFormatter()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var message = Guid.NewGuid().ToString();
            var sensitiveValue = Guid.NewGuid().ToString();
            var categoryName = Guid.NewGuid().ToString();
            var config = new LoggingConfig().Set(x => x.SensitiveValues.Add(sensitiveValue));

            var state = Model.Create<StructuredData>().Set(x => x.FirstName += " " + sensitiveValue);

            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLogger(categoryName, output, config);

            using (sut.BeginScope(state))
            {
                sut.Log(logLevel, eventId, message);
            }

            output.DidNotReceive().WriteLine(Arg.Is<string>(x => x.Contains(sensitiveValue)));
        }
    }
}