namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using NSubstitute;

    public class TestOutputLoggerProviderTests
    {
        [Fact]
        public void CanDisposeMultipleTimesTest()
        {
            var output = Substitute.For<ITestOutputHelper>();

            using (var sut = new TestOutputLoggerProvider(output))
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

            using (var sut = new TestOutputLoggerProvider(output))
            {
                var actual = sut.CreateLogger(categoryName);

                actual.Should().BeOfType<TestOutputLogger>();
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateLoggerThrowsExceptionWithInvalidCategoryNameTest(string categoryName)
        {
            var output = Substitute.For<ITestOutputHelper>();

            using (var sut = new TestOutputLoggerProvider(output))
            {
                Action action = () => sut.CreateLogger(categoryName);

                action.Should().Throw<ArgumentException>();
            }
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOutputTest()
        {
            Action action = () => new TestOutputLoggerProvider(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullOutputTest()
        {
            Action action = () => new TestOutputLoggerProvider(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}