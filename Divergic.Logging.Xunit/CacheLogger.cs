namespace Divergic.Logging.Xunit
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using EnsureThat;
    using Microsoft.Extensions.Logging;

    public class CacheLogger : ICacheLogger
    {
        private readonly IList<LogEntry> _logEntries = new List<LogEntry>();
        private readonly ILogger _logger;

        /// <summary>
        ///     Creates a new instance of the <see cref="CacheLogger" /> class.
        /// </summary>
        public CacheLogger()
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="CacheLogger" /> class.
        /// </summary>
        /// <param name="logger">The source logger.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="logger"/> is <c>null</c>.</exception>
        public CacheLogger(ILogger logger)
        {
            Ensure.Any.IsNotNull(logger, nameof(logger));

            _logger = logger;
        }

        /// <inheritdoc />
        public IDisposable BeginScope<TState>(TState state)
        {
            if (_logger == null)
            {
                return NoopDisposable.Instance;
            }

            return _logger.BeginScope(state);
        }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            if (_logger == null)
            {
                return true;
            }

            return _logger.IsEnabled(logLevel);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="formatter"/> is <c>null</c>.</exception>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Ensure.Any.IsNotNull(formatter, nameof(formatter));

            if (IsEnabled(logLevel) == false)
            {
                return;
            }

            var message = formatter(state, exception);

            if (string.IsNullOrWhiteSpace(message) &&
                exception == null)
            {
                return;
            }

            var entry = new LogEntry(logLevel, eventId, state, exception, message);

            _logEntries.Add(entry);

            _logger?.Log(logLevel, eventId, state, exception, formatter);
        }

        /// <summary>
        ///     Gets the count of cached log entries.
        /// </summary>
        public int Count => _logEntries.Count;

        /// <summary>
        ///     Gets the cached log entries.
        /// </summary>
        public IReadOnlyCollection<LogEntry> Entries => new ReadOnlyCollection<LogEntry>(_logEntries);

        /// <summary>
        ///     Gets the last entry logged.
        /// </summary>
        public LogEntry Last
        {
            get
            {
                var count = _logEntries.Count;

                if (count == 0)
                {
                    return null;
                }

                return _logEntries[count - 1];
            }
        }
    }
}