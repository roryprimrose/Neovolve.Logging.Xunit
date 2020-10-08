namespace Divergic.Logging.Xunit
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="DefaultFormatter" />
    ///     class provides the default formatting of log messages for xUnit test output.
    /// </summary>
    public class DefaultFormatter : ILogFormatter
    {
        private readonly LoggingConfig _config;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultFormatter" /> class.
        /// </summary>
        /// <param name="config">The logging configuration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="config" /> value is <c>null</c>.</exception>
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
            Exception? exception)
        {
            const string Format = "{0}{1} [{2}]: {3}";
            var padding = new string(' ', scopeLevel * _config.ScopePaddingSpaces);
            var parts = new List<string>(2);

            if (string.IsNullOrWhiteSpace(message) == false)
            {
                var part = string.Format(CultureInfo.InvariantCulture, Format, padding, logLevel, eventId.Id, message);

                parts.Add(part);
            }

            if (exception != null)
            {
                var part = string.Format(
                    CultureInfo.InvariantCulture,
                    Format,
                    padding,
                    logLevel,
                    eventId.Id,
                    exception);

                parts.Add(part);
            }

            return string.Join(Environment.NewLine, parts);
        }
    }
}