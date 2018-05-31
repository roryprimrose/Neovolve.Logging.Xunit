namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using LoggerExtensions = Divergic.Logging.Xunit.LoggerExtensions;

    public class LoggerExtensionsTests
    {
        [Fact]
        public void WithCacheReturnsCacheLoggerTest()
        {
            var source = Substitute.For<ILogger>();

            var sut = source.WithCache();

            sut.Should().BeAssignableTo<ICacheLogger>();
        }

        [Fact]
        public void WithCacheThrowsExceptionWithNullLoggerTest()
        {
            Action action = () => LoggerExtensions.WithCache(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WithCacheTReturnsCacheLoggerTest()
        {
            var source = Substitute.For<ILogger<LoggerExtensionsTests>>();

            var sut = source.WithCache();

            sut.Should().BeAssignableTo<ICacheLogger<LoggerExtensionsTests>>();
        }

        [Fact]
        public void WithCacheTThrowsExceptionWithNullLoggerTest()
        {
            Action action = () => LoggerExtensions.WithCache<LoggerExtensionsTests>(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}