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
        public void BuildLoggerForTReturnsILoggerTTest()
        {
            ILogger<TestOutputHelperExtensionsTests> logger = _output.BuildLoggerFor<TestOutputHelperExtensionsTests>();

            logger.Should().BeAssignableTo<ILogger<TestOutputHelperExtensionsTests>>();
        }

        [Fact]
        public void BuildLoggerForTReturnsUsableLoggerTest()
        {
            var logger = _output.BuildLoggerFor<TestOutputHelperExtensionsTests>();

            logger.LogInformation("Hey, does this work? Check the test trace log.");
        }

        [Fact]
        public void BuildLoggerForTThrowsExceptionWithNullOutputTTest()
        {
            Action action = () => TestOutputHelperExtensions.BuildLoggerFor<TestOutputHelperExtensionsTests>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildLoggerThrowsExceptionWithNullOutputTTest()
        {
            Action action = () => TestOutputHelperExtensions.BuildLogger(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildReturnsILoggerTest()
        {
            var logger = _output.BuildLogger();

            logger.Should().BeAssignableTo<ILogger>();
        }

        [Fact]
        public void BuildReturnsUsableLoggerTest()
        {
            var logger = _output.BuildLogger();

            logger.LogInformation("Hey, does this work? Check the test trace log.");
        }
    }
}