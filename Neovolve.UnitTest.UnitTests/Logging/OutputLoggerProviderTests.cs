using FluentAssertions;
using Neovolve.UnitTest.Logging;
using NSubstitute;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Neovolve.UnitTest.UnitTests.Logging
{
    public class OutputLoggerProviderTests
    {
        [Fact]
        public void CanDisposeMultipleTimesTest()
        {
            var output = Substitute.For<ITestOutputHelper>();

            using (var sut = new OutputLoggerProvider(output))
            {
                sut.Dispose();
                sut.Dispose();
            }
        }

        [Fact]
        public void CreateLoggerReturnsOutputLoggerTest()
        {
            var categoryName = Guid.NewGuid().ToString();

            var output = Substitute.For<ITestOutputHelper>();

            using (var sut = new OutputLoggerProvider(output))
            {
                var actual = sut.CreateLogger(categoryName);

                actual.Should().BeOfType<OutputLogger>();
            }
        }

        [Fact]
        public void ThrowsExceptionWithNullOutputTest()
        {
            Action action = () => new OutputLoggerProvider(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}