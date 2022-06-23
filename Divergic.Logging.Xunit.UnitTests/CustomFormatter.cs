namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using System.Text;
    using Microsoft.Extensions.Logging;

    public class CustomFormatter : ILogFormatter
    {
        public string Format(
            int scopeLevel,
            string categoryName,
            LogLevel logLevel,
            EventId eventId,
            string message,
            Exception? exception)
        {
            var sb = new StringBuilder();

            if (scopeLevel > 0)
            {
                sb.Append(' ', scopeLevel * 2);
            }

            sb.Append($"{GetShortLogLevelString(logLevel)} ");

            if (!string.IsNullOrEmpty(categoryName))
            {
                sb.Append($"{categoryName} ");
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