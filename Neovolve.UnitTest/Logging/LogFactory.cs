namespace Neovolve.UnitTest.Logging
{
    using System.Runtime.CompilerServices;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

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
        public static ILogger BuildLog(ITestOutputHelper output = null, [CallerMemberName] string memberName = null)
        {
            using (var factory = new LoggerFactory())
            {
                using (var provider = new OutputLoggerProvider(output))
                {
                    factory.AddProvider(provider);

                    var logger = factory.CreateLogger(memberName);

                    return logger;
                }
            }
        }
    }
}