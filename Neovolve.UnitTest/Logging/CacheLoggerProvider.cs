namespace Neovolve.UnitTest.Logging
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="CacheLoggerProvider" />
    ///     class is used to provide Xunit logging to <see cref="ILoggerFactory" />.
    /// </summary>
    public class CacheLoggerProvider : ILoggerProvider
    {
        private readonly IList<LogEntry> _logEntries;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CacheLoggerProvider" /> class.
        /// </summary>
        /// <param name="logEntries">The log entries.</param>
        public CacheLoggerProvider(IList<LogEntry> logEntries)
        {
            _logEntries = logEntries;
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return new CacheLogger(_logEntries);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // no-op
        }
    }
}