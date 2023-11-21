namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    public class TestOutputHelperExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public TestOutputHelperExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BuildLoggerFactoryReturnsILoggerFactory()
        {
            var factory = _output.BuildLoggerFactory();

            factory.Should().BeAssignableTo<ILoggerFactory>();
        }

        [Fact]
        public void BuildLoggerFactoryThrowsExceptionWithNullOutputT()
        {
            Action action = () => TestOutputHelperExtensions.BuildLoggerFactory(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildLoggerFactoryWithConfigReturnsILoggerFactory()
        {
            var config = new LoggingConfig();

            var factory = _output.BuildLoggerFactory(config);

            factory.Should().BeAssignableTo<ILoggerFactory>();
        }

        [Fact]
        public void BuildLoggerFactoryWithLogLevelReturnsILoggerFactory()
        {
            var factory = _output.BuildLoggerFactory(LogLevel.Error);

            factory.Should().BeAssignableTo<ILoggerFactory>();
        }

        [Theory]
        [ClassData(typeof(LogLevelDataSet))]
        public void BuildLoggerForTLogsAccordingToLogLevel(LogLevel configuredLevel, LogLevel logLevel, bool isEnabled)
        {
            var logger = _output.BuildLoggerFor<TestOutputHelperExtensionsTests>(configuredLevel);

            logger.Should().BeAssignableTo<ILogger<TestOutputHelperExtensionsTests>>();

            logger.Log(logLevel, "Hey, does this work? Check the test trace log.");

            if (isEnabled)
            {
                logger.Entries.Should().HaveCount(1);
            }
            else
            {
                logger.Entries.Should().BeEmpty();
            }
        }

        [Fact]
        public void BuildLoggerForTReturnsILoggerT()
        {
            var logger = _output.BuildLoggerFor<TestOutputHelperExtensionsTests>();

            logger.Should().BeAssignableTo<ILogger<TestOutputHelperExtensionsTests>>();

            logger.LogInformation("Hey, does this work? Check the test trace log.");

            logger.Entries.Should().HaveCount(1);
        }

        [Fact]
        public void BuildLoggerForTReturnsUsableLogger()
        {
            var logger = _output.BuildLoggerFor<TestOutputHelperExtensionsTests>();

            logger.LogInformation("Hey, does this work? Check the test trace log.");

            logger.Entries.Should().HaveCount(1);
        }

        [Fact]
        public void BuildLoggerForTThrowsExceptionWithNullOutputT()
        {
            Action action = () => TestOutputHelperExtensions.BuildLoggerFor<TestOutputHelperExtensionsTests>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildLoggerForTWithLoggingConfigReturnsUsableLogger()
        {
            var config = new LoggingConfig();

            var logger = _output.BuildLoggerFor<TestOutputHelperExtensionsTests>(config);

            logger.LogInformation("Hey, does this work? Check the test trace log.");

            logger.Entries.Should().HaveCount(1);
        }

        [Theory]
        [ClassData(typeof(LogLevelDataSet))]
        public void BuildLoggerLogsAccordingToLogLevel(LogLevel configuredLevel, LogLevel logLevel, bool isEnabled)
        {
            var logger = _output.BuildLogger(configuredLevel);

            logger.Should().BeAssignableTo<ILogger>();

            logger.Log(logLevel, "Hey, does this work? Check the test trace log.");

            if (isEnabled)
            {
                logger.Entries.Should().HaveCount(1);
            }
            else
            {
                logger.Entries.Should().BeEmpty();
            }
        }

        [Fact]
        public void BuildLoggerReturnsILogger()
        {
            var logger = _output.BuildLogger();

            logger.Should().BeAssignableTo<ILogger>();

            logger.LogInformation("Hey, does this work? Check the test trace log.");

            logger.Entries.Should().HaveCount(1);
        }

        [Fact]
        public void BuildLoggerReturnsUsableLogger()
        {
            var logger = _output.BuildLogger();

            logger.LogInformation("Hey, does this work? Check the test trace log.");

            logger.Entries.Should().HaveCount(1);
        }

        [Fact]
        public void BuildLoggerThrowsExceptionWithNullOutputT()
        {
            Action action = () => TestOutputHelperExtensions.BuildLogger(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildLoggerWithLoggingConfigReturnsUsableLogger()
        {
            var config = new LoggingConfig();

            var logger = _output.BuildLogger(config);

            logger.LogInformation("Hey, does this work? Check the test trace log.");

            logger.Entries.Should().HaveCount(1);
        }
    }
}