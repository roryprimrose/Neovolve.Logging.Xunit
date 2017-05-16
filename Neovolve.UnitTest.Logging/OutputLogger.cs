namespace Neovolve.UnitTest.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    public class OutputLogger : ILogger
    {
        private readonly string _name;
        private readonly ITestOutputHelper _output;

        public OutputLogger(string name, ITestOutputHelper output)
        {
            _name = name;
            _output = output;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var formattedMessage = formatter(state, exception);

            if (!string.IsNullOrEmpty(formattedMessage) ||
                exception != null)
            {
                WriteMessage(logLevel, _name, eventId.Id, formattedMessage, exception);
            }
        }

        private void WriteMessage(
            LogLevel logLevel,
            string logName,
            int eventId,
            string message,
            Exception exception)
        {
            const string Format = "{1} [{2}]: {3}";

            if (string.IsNullOrEmpty(message) == false)
            {
                _output.WriteLine(Format, logName, logLevel, eventId, message);
            }

            if (exception != null)
            {
                _output.WriteLine(Format, logName, logLevel, eventId, exception);
            }
        }

        private class NoopDisposable : IDisposable
        {
            // Fields
            public static readonly NoopDisposable Instance = new NoopDisposable();

            // Methods
            public void Dispose()
            {
            }
        }
    }
}