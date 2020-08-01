namespace Divergic.Logging.Xunit
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="CacheLogger{T}" />
    ///     class provides a cache logger for <see cref="ILogger{TCategoryName}" />.
    /// </summary>
    /// <typeparam name="T">The generic type of logger.</typeparam>
    public class CacheLogger<T> : CacheLogger, ICacheLogger<T>
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CacheLogger{T}" /> class.
        /// </summary>
        public CacheLogger()
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="CacheLogger{T}" /> class.
        /// </summary>
        /// <param name="logger">The source logger.</param>
        /// <param name="factory">The logger factory.</param>
        public CacheLogger(ILogger logger, ILoggerFactory factory)
            : base(logger, factory)
        {
        }
    }
}