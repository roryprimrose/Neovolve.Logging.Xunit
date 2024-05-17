namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using LoggerExtensions = LoggerExtensions;

    public class LoggerExtensionsTests
    {
        [Fact]
        public void WithCacheReturnsCacheLogger()
        {
            var source = Substitute.For<ILogger>();

            var sut = source.WithCache();

            sut.Should().BeAssignableTo<ICacheLogger>();
        }

        [Fact]
        public void WithCacheThrowsExceptionWithNullLogger()
        {
            Action action = () => LoggerExtensions.WithCache(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WithCacheTReturnsCacheLogger()
        {
            var source = Substitute.For<ILogger<LoggerExtensionsTests>>();

            var sut = source.WithCache();

            sut.Should().BeAssignableTo<ICacheLogger<LoggerExtensionsTests>>();
        }

        [Fact]
        public void WithCacheTThrowsExceptionWithNullLogger()
        {
            Action action = () => LoggerExtensions.WithCache<LoggerExtensionsTests>(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}