namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using NSubstitute;

    public class LoggerFactoryExtensionsTests
    {
        [Fact]
        public void AddXunitAddsProviderToFactory()
        {
            var output = Substitute.For<ITestOutputHelper>();

            var sut = Substitute.For<ILoggerFactory>();

            sut.AddXunit(output);

            sut.Received().AddProvider(Arg.Is<ILoggerProvider>(x => x is TestOutputLoggerProvider));
        }

        [Fact]
        public void AddXunitThrowsExceptionWithNullFactory()
        {
            var output = Substitute.For<ITestOutputHelper>();

            var sut = (ILoggerFactory)null!;

            Action action = () => sut.AddXunit(output);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddXunitThrowsExceptionWithNullTestOutput()
        {
            var sut = Substitute.For<ILoggerFactory>();

            Action action = () => sut.AddXunit(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddXunitWithLoggingConfigAddsProviderToFactory()
        {
            var config = new LoggingConfig();

            var output = Substitute.For<ITestOutputHelper>();

            var sut = Substitute.For<ILoggerFactory>();

            sut.AddXunit(output, config);

            sut.Received().AddProvider(Arg.Is<ILoggerProvider>(x => x is TestOutputLoggerProvider));
        }
    }
}