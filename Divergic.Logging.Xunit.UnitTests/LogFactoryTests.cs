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
        public void BuildLogForReturnsCacheLoggerT()
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
        public void BuildLogForThrowsExceptionWithNullOutput()
        {
            Action action = () => LogFactory.BuildLogFor<LogFactoryTests>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildLogForWithCustomFormatterReturnsCacheLoggerT()
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
        public void BuildLogReturnsCacheLogger()
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
        public void BuildLogThrowsExceptionWithNullOutput()
        {
            Action action = () => LogFactory.BuildLog(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildLogWithCustomFormatterReturnsCacheLogger()
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
        public void CreateReturnsFactory()
        {
            using (var sut = LogFactory.Create(_output))
            {
                var logger = sut.CreateLogger<LogFactoryTests>();

                logger.LogInformation("This should be written to the test out");
            }
        }

        [Fact]
        public void CreateThrowsExceptionWithNullOutput()
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
        public void CreateWithCustomFormatterReturnsFactory()
        {
            using (var sut = LogFactory.Create(_output, Formatters.MyCustomFormatter))
            {
                var logger = sut.CreateLogger<LogFactoryTests>();

                logger.LogInformation("This should be written to the test out");
            }
        }
    }
}