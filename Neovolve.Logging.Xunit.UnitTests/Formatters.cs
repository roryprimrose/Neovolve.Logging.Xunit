namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using Microsoft.Extensions.Logging;

    internal static class Formatters
    {
        // This an example message formatter.
        public static string MyCustomFormatter(
            int scopeLevel,
            string categoryName,
            LogLevel logLevel,
            EventId eventId,
            string message,
            Exception exception)
        {
            var formatter = new CustomFormatter();

            return formatter.Format(scopeLevel, categoryName, logLevel, eventId, message, exception);
        }
    }
}