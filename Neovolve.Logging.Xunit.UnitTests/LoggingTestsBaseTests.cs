namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using Microsoft.Extensions.Logging;

    public class LoggingTestsBaseTests
    {
        private readonly ITestOutputHelper _output;

        public LoggingTestsBaseTests(ITestOutputHelper output)
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

            using var sut = new Wrapper(_output, config);

            sut.OutputValue.Should().Be(_output);
            sut.LoggerValue.Should().NotBeNull();
        }

        [Fact]
        public void ReturnsLoggerAndOutputWithLogLevel()
        {
            using var sut = new Wrapper(_output, LogLevel.Error);

            sut.OutputValue.Should().Be(_output);
            sut.LoggerValue.Should().NotBeNull();
        }

        [Fact]
        public void ReturnsLoggerAndOutputWithNullConfiguration()
        {
            using var sut = new Wrapper(_output, null);

            sut.OutputValue.Should().Be(_output);
            sut.LoggerValue.Should().NotBeNull();
        }

        [Fact]
        public void ReturnsLoggerAndOutputWithoutConfiguration()
        {
            using var sut = new Wrapper(_output);

            sut.OutputValue.Should().Be(_output);
            sut.LoggerValue.Should().NotBeNull();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOutput()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : LoggingTestsBase
        {
            public Wrapper(ITestOutputHelper output) : base(output)
            {
            }

            public Wrapper(ITestOutputHelper output, LogLevel logLevel) : base(output, logLevel)
            {
            }

            public Wrapper(ITestOutputHelper output, LoggingConfig? config = null) : base(output, config)
            {
            }

            public ILogger LoggerValue => Logger;

            public ITestOutputHelper OutputValue => Output;
        }
    }
}