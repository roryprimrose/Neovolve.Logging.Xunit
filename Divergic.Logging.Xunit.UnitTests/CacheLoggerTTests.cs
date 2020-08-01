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

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new CacheLogger<CacheLoggerTTests>(source);

            action.Should().NotThrow();
        }

        [Fact]
        public void ThrowsExceptionWithNullSourceLogger()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new CacheLogger<CacheLoggerTTests>(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}