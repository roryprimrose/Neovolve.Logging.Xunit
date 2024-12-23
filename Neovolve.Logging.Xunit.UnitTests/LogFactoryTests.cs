namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using Microsoft.Extensions.Logging;

    public class LogFactoryTests
    {
        private readonly ITestOutputHelper _output;

        public LogFactoryTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CreateReturnsFactory()
        {
            using var sut = LogFactory.Create(_output);
            var logger = sut.CreateLogger<LogFactoryTests>();

            logger.LogInformation("This should be written to the test out");
        }

        [Fact]
        public void CreateThrowsExceptionWithNullOutput()
        {
            var action = () =>
            {
                using (LogFactory.Create(null!))
                {
                }
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithLoggingConfigReturnsFactory()
        {
            var config = new LoggingConfig();

            using var sut = LogFactory.Create(_output, config);
            var logger = sut.CreateLogger<LogFactoryTests>();

            logger.LogInformation("This should be written to the test out");
        }
    }
}