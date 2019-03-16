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
        public static ICacheLogger BuildLog(ITestOutputHelper output, [CallerMemberName] string memberName = null)
        {
            Ensure.Any.IsNotNull(output, nameof(output));

            using (var factory = Create(output))
            {
                var logger = factory.CreateLogger(memberName);

                return logger.WithCache();
            }
        }

        /// <summary>
        ///     Builds a logger using the specified output.
        /// </summary>
        /// <typeparam name="T">The type to create the logger for.</typeparam>
        /// <param name="output">The test output logger.</param>
        /// <returns>The logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public static ICacheLogger<T> BuildLogFor<T>(ITestOutputHelper output)
        {
            Ensure.Any.IsNotNull(output, nameof(output));

            using (var factory = Create(output))
            {
                var logger = factory.CreateLogger<T>();

                return logger.WithCache();
            }
        }

        /// <summary>
        ///     Creates an <see cref="ILoggerFactory" /> instance that is configured for xUnit output.
        /// </summary>
        /// <param name="output">The test output.</param>
        /// <returns>The logger factory.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public static ILoggerFactory Create(ITestOutputHelper output)
        {
            Ensure.Any.IsNotNull(output, nameof(output));

            var factory = new LoggerFactory();

            factory.AddXunit(output);

            return factory;
        }
    }
}