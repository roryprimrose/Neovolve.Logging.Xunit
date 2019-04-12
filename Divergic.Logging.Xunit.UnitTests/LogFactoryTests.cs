namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;

    public class LogFactoryTests
    {
        private readonly ITestOutputHelper _output;

        public LogFactoryTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BuildLogForReturnsCacheLoggerTTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => data;

            var sut = LogFactory.BuildLogFor<LogFactoryTests>(_output);

            sut.Log(logLevel, eventId, state, exception, formatter);

            sut.Should().BeAssignableTo<ICacheLogger<LogFactoryTests>>();
            sut.Count.Should().Be(1);
            sut.Last.EventId.Should().Be(eventId);
            sut.Last.Exception.Should().Be(exception);
            sut.Last.LogLevel.Should().Be(logLevel);
            sut.Last.Scopes.Should().BeEmpty();
            sut.Last.State.Should().Be(state);
            sut.Last.Message.Should().Be(data);
        }

        [Fact]
        public void BuildLogForThrowsExceptionWithNullOutputTest()
        {
            Action action = () => LogFactory.BuildLogFor<LogFactoryTests>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildLogForWithCustomFormatterReturnsCacheLoggerTTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => data;

            var sut = LogFactory.BuildLogFor<LogFactoryTests>(_output, Formatters.MyCustomFormatter);

            sut.Log(logLevel, eventId, state, exception, formatter);

            sut.Should().BeAssignableTo<ICacheLogger<LogFactoryTests>>();
            sut.Count.Should().Be(1);
            sut.Last.EventId.Should().Be(eventId);
            sut.Last.Exception.Should().Be(exception);
            sut.Last.LogLevel.Should().Be(logLevel);
            sut.Last.Scopes.Should().BeEmpty();
            sut.Last.State.Should().Be(state);
            sut.Last.Message.Should().Be(data);
        }

        [Fact]
        public void BuildLogReturnsCacheLoggerTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => data;

            var sut = LogFactory.BuildLog(_output);

            sut.Log(logLevel, eventId, state, exception, formatter);

            sut.Should().BeAssignableTo<ICacheLogger>();
            sut.Count.Should().Be(1);
            sut.Last.EventId.Should().Be(eventId);
            sut.Last.Exception.Should().Be(exception);
            sut.Last.LogLevel.Should().Be(logLevel);
            sut.Last.Scopes.Should().BeEmpty();
            sut.Last.State.Should().Be(state);
            sut.Last.Message.Should().Be(data);
        }

        [Fact]
        public void BuildLogThrowsExceptionWithNullOutputTest()
        {
            Action action = () => LogFactory.BuildLog(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildLogWithCustomFormatterReturnsCacheLoggerTest()
        {
            var logLevel = LogLevel.Error;
            var eventId = Model.Create<EventId>();
            var state = Guid.NewGuid().ToString();
            var data = Guid.NewGuid().ToString();
            var exception = new ArgumentNullException(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Func<string, Exception, string> formatter = (message, error) => data;

            var sut = LogFactory.BuildLog(_output, Formatters.MyCustomFormatter);

            sut.Log(logLevel, eventId, state, exception, formatter);

            sut.Should().BeAssignableTo<ICacheLogger>();
            sut.Count.Should().Be(1);
            sut.Last.EventId.Should().Be(eventId);
            sut.Last.Exception.Should().Be(exception);
            sut.Last.LogLevel.Should().Be(logLevel);
            sut.Last.Scopes.Should().BeEmpty();
            sut.Last.State.Should().Be(state);
            sut.Last.Message.Should().Be(data);
        }

        [Fact]
        public void CreateReturnsFactoryTest()
        {
            using (var sut = LogFactory.Create(_output))
            {
                var logger = sut.CreateLogger<LogFactoryTests>();

                logger.LogInformation("This should be written to the test out");
            }
        }

        [Fact]
        public void CreateThrowsExceptionWithNullOutputTest()
        {
            Action action = () =>
            {
                using (LogFactory.Create(null))
                {
                }
            };

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithCustomFormatterReturnsFactoryTest()
        {
            using (var sut = LogFactory.Create(_output, Formatters.MyCustomFormatter))
            {
                var logger = sut.CreateLogger<LogFactoryTests>();

                logger.LogInformation("This should be written to the test out");
            }
        }
    }
}