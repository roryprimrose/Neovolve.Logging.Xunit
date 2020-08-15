namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using LoggerExtensions = Xunit.LoggerExtensions;

    public class LoggerExtensionsTests
    {
        [Fact]
        public void WithCacheReturnsCacheLogger()
        {
            var source = Substitute.For<ILogger>();
            var factory = Substitute.For<ILoggerFactory>();

            var sut = source.WithCache(factory);

            sut.Should().BeAssignableTo<ICacheLogger>();
        }

        [Fact]
        public void WithCacheThrowsExceptionWithNullFactory()
        {
            var source = Substitute.For<ILogger>();

            Action action = () => source.WithCache(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WithCacheThrowsExceptionWithNullLogger()
        {
            var factory = Substitute.For<ILoggerFactory>();

            Action action = () => LoggerExtensions.WithCache(null!, factory);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WithCacheTReturnsCacheLogger()
        {
            var factory = Substitute.For<ILoggerFactory>();

            var source = Substitute.For<ILogger<LoggerExtensionsTests>>();

            var sut = source.WithCache(factory);

            sut.Should().BeAssignableTo<ICacheLogger<LoggerExtensionsTests>>();
        }

        [Fact]
        public void WithCacheTThrowsExceptionWithNullFactory()
        {
            var source = Substitute.For<ILogger<LoggerExtensionsTests>>();

            Action action = () => source.WithCache(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WithCacheTThrowsExceptionWithNullLogger()
        {
            var factory = Substitute.For<ILoggerFactory>();

            Action action = () => LoggerExtensions.WithCache<LoggerExtensionsTests>(null!, factory);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}