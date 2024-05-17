namespace Neovolve.Logging.Xunit
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="CacheLogger" />
    ///     class provides a cache of log entries written to the logger.
    /// </summary>
    public class CacheLogger : FilterLogger, ICacheLogger
    {
        private static readonly AsyncLocal<ConcurrentStack<CacheScope>> _scopes = new();

        private readonly ILoggerFactory? _factory;
        private readonly ConcurrentStack<LogEntry> _logEntries = new();
        private readonly ILogger? _logger;

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
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        public CacheLogger(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        internal CacheLogger(ILogger logger, ILoggerFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        /// <inheritdoc />
        public override IDisposable? BeginScope<TState>(TState state)
        {
            var scope = _logger?.BeginScope(state) ?? NoopDisposable.Instance;

            var cacheScope = new CacheScope(scope, state, () => Scopes.TryPop(out _));

            Scopes.Push(cacheScope);

            return cacheScope;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public override bool IsEnabled(LogLevel logLevel)
        {
            if (_logger == null)
            {
                return true;
            }

            return _logger.IsEnabled(logLevel);
        }

        /// <summary>
        ///     Disposes resources held by this instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> if disposing unmanaged types; otherwise <c>false</c>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _factory?.Dispose();
            }
        }

        /// <inheritdoc />
        protected override void WriteLogEntry<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            string message,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var entry = new LogEntry(
                logLevel,
                eventId,
                state,
                exception,
                message,
                Scopes.Select(s => s.State).ToArray());

            _logEntries.Push(entry);

            _logger?.Log(logLevel, eventId, state, exception, formatter);
        }

        /// <summary>
        ///     Gets the count of cached log entries.
        /// </summary>
        public int Count => _logEntries.Count;

        /// <summary>
        ///     Gets the cached log entries.
        /// </summary>
        public IReadOnlyCollection<LogEntry> Entries => _logEntries.Reverse().ToList();

        /// <summary>
        ///     Gets the last entry logged.
        /// </summary>
        public LogEntry? Last
        {
            get
            {
                if (_logEntries.TryPeek(out var value))
                {
                    return value;
                }

                return null;
            }
        }

        private static ConcurrentStack<CacheScope> Scopes
        {
            get
            {
                var scopes = _scopes.Value;

                if (scopes == null)
                {
                    scopes = new();

                    _scopes.Value = scopes;
                }

                return scopes;
            }
        }
    }
}