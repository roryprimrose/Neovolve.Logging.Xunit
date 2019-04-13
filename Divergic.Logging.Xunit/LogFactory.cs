namespace Divergic.Logging.Xunit
{
    using System;
    using System.Runtime.CompilerServices;
    using EnsureThat;
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
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        [Obsolete("This method will be removed in the next version. Use output.BuildLogger() instead.")]
        public static ICacheLogger BuildLog(ITestOutputHelper output, [CallerMemberName] string memberName = null)
        {
            return BuildLog(output, null, memberName);
        }

        /// <summary>
        ///     Builds a logger using the specified output.
        /// </summary>
        /// <param name="output">The test output logger.</param>
        /// <param name="customFormatter">Optional custom message formatter.</param>
        /// <param name="memberName">
        ///     The member to create the logger for. This is automatically populated using
        ///     <see cref="CallerMemberNameAttribute" />.
        /// </param>
        /// <returns>The logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        [Obsolete("This method will be removed in the next version. Use output.BuildLogger(customFormatter) instead.")]
        public static ICacheLogger BuildLog(
            ITestOutputHelper output,
            Func<int, string, LogLevel, EventId, string, Exception, string> customFormatter,
            [CallerMemberName] string memberName = null)
        {
            return output.BuildLogger(customFormatter, memberName);
        }

        /// <summary>
        ///     Builds a logger using the specified output.
        /// </summary>
        /// <typeparam name="T">The type to create the logger for.</typeparam>
        /// <param name="output">The test output logger.</param>
        /// <param name="customFormatter">Optional custom message formatter.</param>
        /// <returns>The logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        [Obsolete("This method will be removed in the next version. Use output.BuildLoggerFor<T>(customFormatter) instead.")]
        public static ICacheLogger<T> BuildLogFor<T>(
            ITestOutputHelper output,
            Func<int, string, LogLevel, EventId, string, Exception, string> customFormatter = null)
        {
            return output.BuildLoggerFor<T>(customFormatter);
        }

        /// <summary>
        ///     Creates an <see cref="ILoggerFactory" /> instance that is configured for xUnit output.
        /// </summary>
        /// <param name="output">The test output.</param>
        /// <param name="customFormatter">Optional custom message formatter.</param>
        /// <returns>The logger factory.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public static ILoggerFactory Create(
            ITestOutputHelper output,
            Func<int, string, LogLevel, EventId, string, Exception, string> customFormatter = null)
        {
            Ensure.Any.IsNotNull(output, nameof(output));

            var factory = new LoggerFactory();

            factory.AddXunit(output, customFormatter);

            return factory;
        }
    }
}