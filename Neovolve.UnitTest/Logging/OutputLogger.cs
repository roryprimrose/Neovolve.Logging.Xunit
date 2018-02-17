namespace Neovolve.UnitTest.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    /// <summary>
    ///     The <see cref="OutputLogger" />
    ///     class is used to provide logging implementation for Xunit.
    /// </summary>
    public class OutputLogger : ILogger
    {
        private readonly string _name;
        private readonly ITestOutputHelper _output;

        /// <summary>
        ///     Creates a new instance of the <see cref="OutputLogger" /> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="output">The test output helper.</param>
        public OutputLogger(string name, ITestOutputHelper output)
        {
            _name = name;
            _output = output;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <inheritdoc />
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

        private void WriteMessage(LogLevel logLevel, string logName, int eventId, string message, Exception exception)
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
            public static readonly NoopDisposable Instance = new NoopDisposable();

            public void Dispose()
            {
                // No-op
            }
        }
    }
}