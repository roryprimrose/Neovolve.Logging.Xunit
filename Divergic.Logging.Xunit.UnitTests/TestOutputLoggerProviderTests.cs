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
        public void CanDisposeMultipleTimes()
        {
            var output = Substitute.For<ITestOutputHelper>();

            using var sut = new TestOutputLoggerProvider(output);
            sut.Dispose();
            sut.Dispose();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateLoggerThrowsExceptionWithInvalidCategoryNameTest(string? categoryName)
        {
            var output = Substitute.For<ITestOutputHelper>();

            using var sut = new TestOutputLoggerProvider(output);

            // ReSharper disable once AccessToDisposedClosure
            Action action = () => sut.CreateLogger(categoryName!);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void CreateLoggerWithLoggingConfigReturnsOutputLogger()
        {
            var categoryName = Guid.NewGuid().ToString();
            var config = new LoggingConfig();

            var output = Substitute.For<ITestOutputHelper>();

            using var sut = new TestOutputLoggerProvider(output, config);
            var actual = sut.CreateLogger(categoryName);

            actual.Should().BeOfType<TestOutputLogger>();
        }

        [Fact]
        public void CreateLoggerWithoutLoggingConfigReturnsOutputLogger()
        {
            var categoryName = Guid.NewGuid().ToString();

            var output = Substitute.For<ITestOutputHelper>();

            using var sut = new TestOutputLoggerProvider(output);
            var actual = sut.CreateLogger(categoryName);

            actual.Should().BeOfType<TestOutputLogger>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullOutput()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new TestOutputLoggerProvider(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}