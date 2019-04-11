using System;
using System.Text;
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
            var sb = new StringBuilder();

            if (scopeLevel > 0)
            {
                sb.Append(' ', scopeLevel * 2);
            }

            sb.Append($"{GetShortLogLevelString(logLevel)} ");

            if (!string.IsNullOrEmpty(name))
            {
                sb.Append($"{name} ");
            }

            if (eventId.Id != 0)
            {
                sb.Append($"[{eventId.Id}]: ");
            }

            if (!string.IsNullOrEmpty(message))
            {
                sb.Append(message);
            }

            if (exception != null)
            {
                sb.Append($"\n{exception}");
            }

            return sb.ToString();
        }

        private static string GetShortLogLevelString(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace: return "Trace";
                case LogLevel.Debug: return "Debug";
                case LogLevel.Information: return "Info ";
                case LogLevel.Warning: return "Warn ";
                case LogLevel.Error: return "Error";
                case LogLevel.Critical: return "Crit ";
                case LogLevel.None: return "None ";
                default: throw new Exception("invalid");
            }
        }
    }
}