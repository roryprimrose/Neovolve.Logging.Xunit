namespace Xunit.Abstractions
{
    using System;
    using System.Runtime.CompilerServices;
    using Neovolve.UnitTest.Logging;

    /// <summary>
    ///     The <see cref="TestOutputHelperExtensions" />
    ///     class provides extension methods for the <see cref="ITestOutputHelper" />.
    /// </summary>
    public static class TestOutputHelperExtensions
    {
        /// <summary>
        ///     Builds a logger from the specified test output helper.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        /// <param name="memberName">
        ///     The member to create the logger for. This is automatically populated using
        ///     <see cref="CallerMemberNameAttribute" />.
        /// </param>
        /// <returns>The logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public static ICacheLogger BuildLogger(this ITestOutputHelper output, [CallerMemberName] string memberName = "")
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return LogFactory.BuildLog(output, memberName);
        }

        /// <summary>
        ///     Builds a logger from the specified test output helper for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to create the logger for.</typeparam>
        /// <param name="output">The test output helper.</param>
        /// <returns>The logger.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public static ICacheLogger BuildLoggerFor<T>(this ITestOutputHelper output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return LogFactory.BuildLogFor<T>(output);
        }
    }
}