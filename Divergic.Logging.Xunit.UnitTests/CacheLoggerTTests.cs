namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using Microsoft.Extensions.Logging;
    using NSubstitute;

    public class CacheLoggerTTests
    {
        [Fact]
        public void CanCreate()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new CacheLogger<CacheLoggerTTests>();

            action.Should().NotThrow();
        }

        [Fact]
        public void CanCreateWithSourceLogger()
        {
            var source = Substitute.For<ILogger<CacheLoggerTTests>>();
            var factory = Substitute.For<ILoggerFactory>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new CacheLogger<CacheLoggerTTests>(source, factory);

            action.Should().NotThrow();
        }

        [Fact]
        public void ThrowsExceptionWithNullFactory()
        {
            var source = Substitute.For<ILogger<CacheLoggerTTests>>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new CacheLogger<CacheLoggerTTests>(source, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullSourceLogger()
        {
            var factory = Substitute.For<ILoggerFactory>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new CacheLogger<CacheLoggerTTests>(null, factory);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}