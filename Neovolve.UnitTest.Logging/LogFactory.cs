namespace Neovolve.UnitTest.Logging
{
    using System.Runtime.CompilerServices;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    public static class LogFactory
    {
        public static ILogger BuildLog(ITestOutputHelper output = null, [CallerMemberName] string memberName = "")
        {
            using (var factory = new Microsoft.Extensions.Logging.LoggerFactory())
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