namespace Divergic.Logging.Xunit
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="LogFactory" />
    ///     class is used to create <see cref="ILogger" /> instances.
    /// </summary>
    public static class LogFactory
    {
        /// <summary>
        ///     Builds a logger using the specified output.
        /// </summary>
        /// <param name="output">The test output logger.</param>
        /// <param name="memberName">
        ///     The member to create the logger for. This is automatically populated using
        ///     <see cref="CallerMemberNameAttribute" />.
        /// </param>
        /// <returns>The logger.</returns>
        public static ICacheLogger BuildLog(ITestOutputHelper output = null,
            [CallerMemberName] string memberName = null)
        {
            var logEntries = new List<LogEntry>();

            using (var factory = new LoggerFactory())
            using (var cacheProvider = new CacheLoggerProvider(logEntries))
            using (var outputProvider = new OutputLoggerProvider(output))
            {
                factory.AddProvider(cacheProvider);
                factory.AddProvider(outputProvider);

                var logger = factory.CreateLogger(memberName);

                var cacheLogger = new CacheLoggerWrapper(logger, logEntries);

                return cacheLogger;
            }
        }

        /// <summary>
        ///     Builds a logger using the specified output.
        /// </summary>
        /// <typeparam name="T">The type to create the logger for.</typeparam>
        /// <param name="output">The test output logger.</param>
        /// <returns>The logger.</returns>
        public static ICacheLogger<T> BuildLogFor<T>(ITestOutputHelper output = null)
        {
            var logEntries = new List<LogEntry>();

            using (var factory = new LoggerFactory())
            using (var cacheProvider = new CacheLoggerProvider(logEntries))
            using (var outputProvider = new OutputLoggerProvider(output))
            {
                factory.AddProvider(cacheProvider);
                factory.AddProvider(outputProvider);

                var logger = factory.CreateLogger<T>();

                var cacheLogger = new CacheLoggerWrapper<T>(logger, logEntries);

                return cacheLogger;
            }
        }
    }
}