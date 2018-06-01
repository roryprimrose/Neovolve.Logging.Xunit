namespace Divergic.Logging.Xunit.UnitTests
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
        public void AddXunitAddsProviderToFactoryTest()
        {
            var output = Substitute.For<ITestOutputHelper>();

            var sut = Substitute.For<ILoggerFactory>();

            sut.UseXunit(output);

            sut.Received().AddProvider(Arg.Is<ILoggerProvider>(x => x is TestOutputLoggerProvider));
        }

        [Fact]
        public void AddXunitThrowsExceptionWithNullFactoryTest()
        {
            var output = Substitute.For<ITestOutputHelper>();

            var sut = (ILoggerFactory) null;

            Action action = () => sut.UseXunit(output);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddXunitThrowsExceptionWithNullTestOutputTest()
        {
            var sut = Substitute.For<ILoggerFactory>();

            Action action = () => sut.UseXunit(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}