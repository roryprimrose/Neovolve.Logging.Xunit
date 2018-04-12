namespace Neovolve.UnitTest.UnitTests.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Xunit;
    using Xunit.Abstractions;

    public class LogFactoryTests
    {
        private readonly ITestOutputHelper _output;

        public LogFactoryTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static List<object[]> ResolveLevels()
        {
            var values = Enum.GetValues(typeof(LogLevel));
            var results = new List<object[]>(values.Length);

            foreach (var value in values)
            {
                var result = new[]
                    {value};

                results.Add(result);
            }

            return results;
        }

        [Fact]
        public void BuildLoggerForTypeCachesLogEntriesTests()
        {
            var logLevel = LogLevel.Information;
            var eventId = new EventId(Environment.TickCount);
            var state = new
            { Name = Guid.NewGuid().ToString() };
            var exception = GetThrownException();

            var actual = _output.BuildLoggerFor<LogFactoryTests>();

            actual.Count.Should().Be(0);
            actual.Latest.Should().BeNull();

            actual.Log(logLevel, eventId, state, exception, (data, ex) => state.ToString());

            actual.Count.Should().Be(1);

            var latest = actual.Latest;

            latest.Should().NotBeNull();
            latest.LogLevel.Should().Be(logLevel);
            latest.EventId.Should().Be(eventId);
            latest.State.Should().Be(state);
            latest.Exception.Should().Be(exception);
            latest.Message.Should().NotBeNullOrWhiteSpace();

            var entries = actual.Entries;

            entries.Count.Should().Be(actual.Count);

            var entry = entries.First();

            latest.Should().BeEquivalentTo(entry);
        }

        [Fact]
        public void BuildLoggerForTypeCanBeginScopeTests()
        {
            var state = new
            { Name = Guid.NewGuid().ToString() };

            var actual = _output.BuildLoggerFor<LogFactoryTests>();

            using (var scope = actual.BeginScope(state))
            {
                scope.Dispose();
            }
        }

        [Theory]
        [InlineData(LogLevel.Critical)]
        [InlineData(LogLevel.Debug)]
        [InlineData(LogLevel.Error)]
        [InlineData(LogLevel.Information)]
        [InlineData(LogLevel.None)]
        [InlineData(LogLevel.Trace)]
        [InlineData(LogLevel.Warning)]
        public void BuildLoggerForTypeReturnsLoggerWithIsEnabledReturnsTrueTest(LogLevel logLevel)
        {
            var state = new
            { Name = Guid.NewGuid().ToString() };

            var logger = _output.BuildLoggerFor<LogFactoryTests>();

            var actual = logger.IsEnabled(logLevel);

            actual.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(ResolveLevels), MemberType = typeof(LogFactoryTests))]
        public void BuildLoggerForTypeTests(LogLevel level)
        {
            var eventId = new EventId(Environment.TickCount);
            var state = new
            { Name = Guid.NewGuid().ToString() };
            var exception = GetThrownException();

            var actual = _output.BuildLoggerFor<LogFactoryTests>();

            actual.Log(level, eventId, state, exception, (data, ex) => state.ToString());
        }

        [Theory]
        [MemberData(nameof(ResolveLevels), MemberType = typeof(LogFactoryTests))]
        public void BuildLoggerTests(LogLevel level)
        {
            var eventId = new EventId(Environment.TickCount);
            var state = new
            { Name = Guid.NewGuid().ToString() };
            var exception = GetThrownException();

            var actual = _output.BuildLogger();

            actual.Log(level, eventId, state, exception, (data, ex) => state.ToString());
        }

        private static Exception GetThrownException()
        {
            try
            {
                throw new TimeoutException();
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }
}