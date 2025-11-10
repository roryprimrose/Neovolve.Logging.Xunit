namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
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

        [Fact]
        public void CreateLoggerReturnsCachedLoggerForSameCategoryName()
        {
            var categoryName = Guid.NewGuid().ToString();

            var output = Substitute.For<ITestOutputHelper>();

            using var sut = new TestOutputLoggerProvider(output);
            var first = sut.CreateLogger(categoryName);
            var second = sut.CreateLogger(categoryName);

            first.Should().BeSameAs(second);
        }

        [Fact]
        public void CreateLoggerReturnsDifferentLoggersForDifferentCategoryNames()
        {
            var firstCategory = Guid.NewGuid().ToString();
            var secondCategory = Guid.NewGuid().ToString();

            var output = Substitute.For<ITestOutputHelper>();

            using var sut = new TestOutputLoggerProvider(output);
            var first = sut.CreateLogger(firstCategory);
            var second = sut.CreateLogger(secondCategory);

            first.Should().NotBeSameAs(second);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateLoggerThrowsExceptionWithInvalidCategoryName(string? categoryName)
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
        public void DisposeClearsCachedLoggers()
        {
            var categoryName = Guid.NewGuid().ToString();
            var output = Substitute.For<ITestOutputHelper>();

            var sut = new TestOutputLoggerProvider(output);
            var firstLogger = sut.CreateLogger(categoryName);

            sut.Dispose();

            var secondLogger = sut.CreateLogger(categoryName);

            secondLogger.Should().NotBeSameAs(firstLogger);
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