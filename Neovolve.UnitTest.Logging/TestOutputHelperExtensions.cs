namespace Xunit.Abstractions
{
    using System;
    using System.Runtime.CompilerServices;
    using Microsoft.Extensions.Logging;
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
        public static ILogger BuildLogger(this ITestOutputHelper output, [CallerMemberName] string memberName = "")
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            return LogFactory.BuildLog(output, memberName);
        }
    }
}