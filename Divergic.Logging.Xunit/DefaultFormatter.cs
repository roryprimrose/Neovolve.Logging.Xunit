namespace Divergic.Logging.Xunit
{
    using System;
    using System.Globalization;
    using System.Text;
    using EnsureThat;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="DefaultFormatter" />
    ///     class provides the default formatting of log messages for xUnit test output.
    /// </summary>
    public class DefaultFormatter : ILogFormatter
    {
        private readonly LoggingConfig _config;

        public DefaultFormatter(LoggingConfig config)
        {
            Ensure.Any.IsNotNull(config, nameof(config));

            _config = config;
        }

        /// <inheritdoc />
        public string Format(
            int scopeLevel,
            string name,
            LogLevel logLevel,
            EventId eventId,
            string message,
            Exception exception)
        {
            const string Format = "{0}{2} [{3}]: {4}";
            var padding = new string(' ', scopeLevel * _config.ScopePaddingSpaces);

            var builder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(message) == false)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture,
                    Format,
                    padding,
                    name,
                    logLevel,
                    eventId.Id,
                    message);
                builder.AppendLine();
            }

            if (exception != null)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture,
                    Format,
                    padding,
                    name,
                    logLevel,
                    eventId.Id,
                    exception);
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}