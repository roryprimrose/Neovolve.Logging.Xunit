namespace Divergic.Logging.Xunit
{
    using System;
    using System.Globalization;
    using System.Text;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="DefaultFormatter" />
    ///     class provides the default formatting of log messages for xUnit test output.
    /// </summary>
    public class DefaultFormatter : ILogFormatter
    {
        private readonly LoggingConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFormatter"/> class.
        /// </summary>
        /// <param name="config">The logging configuration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="config"/> value is <c>null</c>.</exception>
        public DefaultFormatter(LoggingConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
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
            const string format = "{0}{2} [{3}]: {4}";
            var padding = new string(' ', scopeLevel * _config.ScopePaddingSpaces);

            var builder = new StringBuilder();

            if (string.IsNullOrWhiteSpace(message) == false)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture,
                    format,
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
                    format,
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