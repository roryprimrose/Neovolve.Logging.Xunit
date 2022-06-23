using System;
using Microsoft.Extensions.Logging;

namespace Divergic.Logging.Xunit.UnitTests
{
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