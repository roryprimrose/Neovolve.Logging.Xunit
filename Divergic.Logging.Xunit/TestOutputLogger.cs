namespace Divergic.Logging.Xunit
{
    using System;
    using EnsureThat;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="TestOutputLogger" />
    ///     class is used to provide logging implementation for Xunit.
    /// </summary>
    public class TestOutputLogger : ILogger
    {
        private readonly string _name;
        private readonly ITestOutputHelper _output;

        /// <summary>
        ///     Creates a new instance of the <see cref="TestOutputLogger" /> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="output">The test output helper.</param>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c>, empty or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public TestOutputLogger(string name, ITestOutputHelper output)
        {
            Ensure.String.IsNotNullOrWhiteSpace(name, nameof(name));
            Ensure.Any.IsNotNull(output, nameof(output));

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
            const string format = "{1} [{2}]: {3}";

            if (string.IsNullOrWhiteSpace(message) == false)
            {
                _output.WriteLine(format, logName, logLevel, eventId, message);
            }

            if (exception != null)
            {
                _output.WriteLine(format, logName, logLevel, eventId, exception);
            }
        }
    }
}