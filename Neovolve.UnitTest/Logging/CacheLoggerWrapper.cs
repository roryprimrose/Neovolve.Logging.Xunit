namespace Neovolve.UnitTest.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Extensions.Logging;

    internal class CacheLoggerWrapper : ICacheLogger
    {
        private readonly IList<LogEntry> _logEntries;
        private readonly ILogger _logger;

        public CacheLoggerWrapper(ILogger logger, IList<LogEntry> logEntries)
        {
            _logger = logger;
            _logEntries = logEntries;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }

        public int Count => _logEntries.Count;

        public IReadOnlyCollection<LogEntry> Entries => new ReadOnlyCollection<LogEntry>(_logEntries);

        public LogEntry Latest
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