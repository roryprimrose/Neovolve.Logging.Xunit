namespace Neovolve.UnitTest.Logging
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="CacheLogger" />
    ///     class is used to provide logging implementation for caching log entries.
    /// </summary>
    public class CacheLogger : ILogger
    {
        private readonly IList<LogEntry> _logEntries;

        /// <summary>
        ///     Creates a new instance of the <see cref="CacheLogger" /> class.
        /// </summary>
        /// <param name="logEntries">The log entries.</param>
        public CacheLogger(IList<LogEntry> logEntries)
        {
            _logEntries = logEntries;
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

            var entry = new LogEntry(logLevel, eventId, state, exception, formattedMessage);

            _logEntries.Add(entry);
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