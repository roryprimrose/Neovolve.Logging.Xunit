namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;

    public class LoggingConfigTests
    {
        [Fact]
        public void CreatesWithDefaultFormatter()
        {
            var sut = new LoggingConfig();

            sut.Formatter.Should().BeOfType<DefaultFormatter>();
        }

        [Fact]
        public void CreatesWithDefaultScopeFormatter()
        {
            var sut = new LoggingConfig();

            sut.ScopeFormatter.Should().BeOfType<DefaultScopeFormatter>();
        }

        [Fact]
        public void CreatesWithIgnoreTestBoundaryExceptionAsFalse()
        {
            var sut = new LoggingConfig();

            sut.IgnoreTestBoundaryException.Should().BeFalse();
        }

        [Fact]
        public void CreatesWithScopePaddingSpacesHavingValue()
        {
            var sut = new LoggingConfig();

            sut.ScopePaddingSpaces.Should().NotBe(0);
        }

        [Fact]
        public void FormatterThrowsExceptionWhenAssignedNull()
        {
            var sut = new LoggingConfig();

            Action action = () => sut.Formatter = null!;

            action.Should().Throw<ArgumentNullException>();
        }
    }
}