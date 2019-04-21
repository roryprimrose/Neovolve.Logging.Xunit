namespace Divergic.Logging.Xunit.UnitTests
{
    using FluentAssertions;
    using global::Xunit;

    public class LoggingConfigTests
    {
        [Fact]
        public void CreatesWithIgnoreTestBoundaryExceptionAsFalse()
        {
            var sut = new LoggingConfig();

            sut.IgnoreTestBoundaryException.Should().BeFalse();
        }
    }
}