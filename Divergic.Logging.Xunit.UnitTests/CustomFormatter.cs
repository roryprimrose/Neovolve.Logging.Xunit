namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using System.Text;
    using Microsoft.Extensions.Logging;

    public class CustomLoggingConfig : LoggingConfig
    {
        public override bool IgnoreTestBoundaryException { get; set; } = true;

        public override string Format(
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
                case LogLevel.Trace: return "Trace\t";
                case LogLevel.Debug: return "Debug\t";
                case LogLevel.Information: return "Info\t";
                case LogLevel.Warning: return "Warn\t";
                case LogLevel.Error: return "Error\t";
                case LogLevel.Critical: return "Crit\t";
                case LogLevel.None: return "None\t";
                default: throw new Exception("invalid\t");
            }
        }
    }
}