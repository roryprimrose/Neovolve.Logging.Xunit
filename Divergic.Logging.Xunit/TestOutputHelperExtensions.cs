namespace Xunit.Abstractions
{
    using System;
    using System.Runtime.CompilerServices;
    using Divergic.Logging.Xunit;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="TestOutputHelperExtensions" /> class provides extension methods for the
    ///     <see cref="ITestOutputHelper" />.
    /// </summary>
    public static class TestOutputHelperExtensions
    {
        /// <summary>
        ///     Builds a logger from the specified test output helper.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        /// <param name="memberName">
        ///     The member to create the logger for. This is automatically populated using <see cref="CallerMemberNameAttribute" />
        ///     .
        /// </param>
        /// <returns>The logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public static ICacheLogger BuildLogger(
            this ITestOutputHelper output,
            [CallerMemberName] string memberName = null)
        {
            return BuildLogger(output, null, memberName);
        }

        /// <summary>
        ///     Builds a logger from the specified test output helper.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        /// <param name="customFormatter">Optional custom message formatter.</param>
        /// <param name="memberName">
        ///     The member to create the logger for. This is automatically populated using <see cref="CallerMemberNameAttribute" />
        ///     .
        /// </param>
        /// <returns>The logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public static ICacheLogger BuildLogger(
            this ITestOutputHelper output,
            Func<int, string, LogLevel, EventId, string, Exception, string> customFormatter,
            [CallerMemberName] string memberName = null)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return LogFactory.BuildLog(output, customFormatter, memberName);
        }

        /// <summary>
        ///     Builds a logger from the specified test output helper for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to create the logger for.</typeparam>
        /// <param name="output">The test output helper.</param>
        /// <returns>The logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public static ICacheLogger<T> BuildLoggerFor<T>(this ITestOutputHelper output)
        {
            return BuildLoggerFor<T>(output, null);
        }

        /// <summary>
        ///     Builds a logger from the specified test output helper for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to create the logger for.</typeparam>
        /// <param name="output">The test output helper.</param>
        /// <param name="customFormatter">Optional custom message formatter.</param>
        /// <returns>The logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public static ICacheLogger<T> BuildLoggerFor<T>(
            this ITestOutputHelper output,
            Func<int, string, LogLevel, EventId, string, Exception, string> customFormatter)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return LogFactory.BuildLogFor<T>(output, customFormatter);
        }
    }
}