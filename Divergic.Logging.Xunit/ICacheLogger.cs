namespace Divergic.Logging.Xunit
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="ICacheLogger" />
    ///     interface defines the members for recording and accessing log entries.
    /// </summary>
    /// <typeparam name="T">The type of class using the cache.</typeparam>
    public interface ICacheLogger<out T> : ICacheLogger, ILogger<T>
    {
    }

    /// <summary>
    ///     The <see cref="ICacheLogger" />
    ///     interface defines the members for recording and accessing log entries.
    /// </summary>
    public interface ICacheLogger : ILogger
    {
        /// <summary>
        ///     Gets the number of cache entries recorded.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets the recorded cache entries.
        /// </summary>
        IReadOnlyCollection<LogEntry> Entries { get; }

        /// <summary>
        ///     Gets the latest cache entry.
        /// </summary>
        LogEntry Latest { get; }
    }
}