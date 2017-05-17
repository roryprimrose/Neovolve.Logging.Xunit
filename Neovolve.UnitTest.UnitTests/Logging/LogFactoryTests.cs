namespace Neovolve.UnitTest.UnitTests.Logging
{
    using System;
    using System.Collections.Generic;
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

        [Theory]
        [MemberData(nameof(ResolveLevels), MemberType = typeof(LogFactoryTests))]
        public void LogTests(LogLevel level)
        {
            var eventId = new EventId(Environment.TickCount);
            var state = new
            {
                Name = Guid.NewGuid().ToString()
            };
            var exception = GetThrownException();
            var log = _output.BuildLogger();
            log.Log(level, eventId, state, exception, (data, ex) => state.ToString());
        }

        public static List<object[]> ResolveLevels()
        {
            var values = Enum.GetValues(typeof(LogLevel));
            var results = new List<object[]>(values.Length);

            foreach (var value in values)
            {
                var result = new[]
                {
                    value
                };

                results.Add(result);
            }

            return results;
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