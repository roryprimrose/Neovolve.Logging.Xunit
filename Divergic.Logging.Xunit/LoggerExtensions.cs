namespace Divergic.Logging.Xunit
{
    using System;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="LoggerExtensions" />
    ///     class provides extension methods for wrapping <see cref="ILogger" /> instances in <see cref="ICacheLogger" />.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        ///     Returns a <see cref="ICacheLogger" /> for the specified logger.
        /// </summary>
        /// <param name="logger">The source logger.</param>
        /// <param name="factory">The logger factory.</param>
        /// <returns>The cache logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="factory" /> is <c>null</c>.</exception>
        public static ICacheLogger WithCache(this ILogger logger, ILoggerFactory factory)
        {
            logger = logger ?? throw new ArgumentNullException(nameof(logger));
            factory = factory ?? throw new ArgumentNullException(nameof(factory));

            var cacheLogger = new CacheLogger(logger, factory);

            return cacheLogger;
        }

        /// <summary>
        ///     Returns a <see cref="ICacheLogger{T}" /> for the specified logger.
        /// </summary>
        /// <typeparam name="T">The type of generic logger.</typeparam>
        /// <param name="logger">The source logger.</param>
        /// <param name="factory">The logger factory.</param>
        /// <returns>The cache logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="logger" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="factory" /> is <c>null</c>.</exception>
        public static ICacheLogger<T> WithCache<T>(this ILogger<T> logger, ILoggerFactory factory)
        {
            logger = logger ?? throw new ArgumentNullException(nameof(logger));
            factory = factory ?? throw new ArgumentNullException(nameof(factory));

            var cacheLogger = new CacheLogger<T>(logger, factory);

            return cacheLogger;
        }
    }
}