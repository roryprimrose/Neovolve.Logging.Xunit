namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    public class LoggingTestsBaseTTests
    {
        private readonly ITestOutputHelper _output;

        public LoggingTestsBaseTTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ReturnsLoggerAndOutputWithConfiguration()
        {
            var config = new LoggingConfig
            {
                LogLevel = LogLevel.Error
            };

            using var sut = new Wrapper<Guid>(_output, config);

            sut.OutputValue.Should().Be(_output);
            sut.LoggerValue.Should().NotBeNull();
        }

        [Fact]
        public void ReturnsLoggerAndOutputWithLogLevel()
        {
            using var sut = new Wrapper<Guid>(_output, LogLevel.Error);

            sut.OutputValue.Should().Be(_output);
            sut.LoggerValue.Should().NotBeNull();
        }

        [Fact]
        public void ReturnsLoggerAndOutputWithNullConfiguration()
        {
            using var sut = new Wrapper<Guid>(_output, null);

            sut.OutputValue.Should().Be(_output);
            sut.LoggerValue.Should().NotBeNull();
        }

        [Fact]
        public void ReturnsLoggerAndOutputWithoutConfiguration()
        {
            using var sut = new Wrapper<Guid>(_output);

            sut.OutputValue.Should().Be(_output);
            sut.LoggerValue.Should().NotBeNull();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOutput()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper<Guid>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper<T> : LoggingTestsBase<T>
        {
            public Wrapper(ITestOutputHelper output) : base(output)
            {
            }

            public Wrapper(ITestOutputHelper output, LogLevel logLevel) : base(output, logLevel)
            {
            }

            public Wrapper(ITestOutputHelper output, LoggingConfig config = null) : base(output, config)
            {
            }

            public ILogger<T> LoggerValue => Logger;

            public ITestOutputHelper OutputValue => Output;
        }
    }
}