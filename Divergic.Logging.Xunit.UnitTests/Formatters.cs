using System;
using Microsoft.Extensions.Logging;

namespace Divergic.Logging.Xunit.UnitTests
{
    internal static class Formatters
    {
        // This an example message formatter.
        public static string MyCustomFormatter(
            int scopeLevel,
            string name,
            LogLevel logLevel,
            EventId eventId,
            string message,
            Exception exception)
        {
            var formatter = new CustomLoggingConfig();

            return formatter.Format(scopeLevel, name, logLevel, eventId, message, exception);
        }
    }
}