namespace Divergic.Logging.Xunit
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using EnsureThat;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="CacheLogger" />
    ///     class provides a cache of log entries written to the logger.
    /// </summary>
    public class CacheLogger : FilterLogger, ICacheLogger
    {
        private readonly IList<LogEntry> _logEntries = new List<LogEntry>();
        private readonly Stack<Scope> _scopes = new Stack<Scope>();
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
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        public CacheLogger(ILogger logger)
        {
            Ensure.Any.IsNotNull(logger, nameof(logger));

            _logger = logger;
        }

        /// <inheritdoc />
        public override IDisposable BeginScope<TState>(TState state)
        {
            IDisposable innerScope = _logger != null ? _logger.BeginScope(state) : NoopDisposable.Instance;

            Scope outerScope = new Scope(this, innerScope, state);

            _scopes.Push(outerScope);

            return outerScope;
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

        /// <inheritdoc />
        protected override void WriteLogEntry<TState>(LogLevel logLevel, EventId eventId, TState state, string message,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            var entry = new LogEntry(logLevel, eventId, state, exception, message, _scopes.Select(s => s.State).ToArray());

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

        private class Scope : IDisposable
        {
            private readonly CacheLogger _cacheLogger;
            private readonly IDisposable _innerScope;
            private bool _isDisposed;

            public Scope(CacheLogger cacheLogger, IDisposable innerScope, object state)
            {
                _cacheLogger = cacheLogger;
                _innerScope = innerScope;
                _isDisposed = false;

                State = state;
            }
            public object State { get; }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                if (_cacheLogger._scopes.Count > 0)
                {
                    _cacheLogger._scopes.Pop();
                }

                _innerScope.Dispose();

                _isDisposed = true;
            }
        }
    }
}