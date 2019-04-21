using Microsoft.Extensions.Logging;
using System;
using System.Globalization;
using System.Text;

namespace Divergic.Logging.Xunit
{
    /// <summary>
    ///     The <see cref="LoggingConfig" />
    ///     class is used to configure how logging operates.
    /// </summary>
    public class LoggingConfig
    {
        /// <summary>
        ///     Gets or sets whether exceptions thrown while logging outside of the test execution will be ignored.
        /// </summary>
        public virtual bool IgnoreTestBoundaryException { get; set; } = false;

        /// <summary>
        ///     Identifies the number of spaces to use for indenting scopes.
        /// </summary>
        public int PaddingSpaces { get; set; } = 3;

        /// <summary>
        ///     Formats the log message with the specified values.
        /// </summary>
        /// <param name="scopeLevel">The number of active logging scopes.</param>
        /// <param name="name">The logger name.</param>
        /// <param name="logLevel">The log level.</param>
        /// <param name="eventId">The event id.</param>
        /// <param name="message">The log message.</param>
        /// <param name="exception">The exception to be logged.</param>
        /// <returns>The formatted log message.</returns>
        public virtual string Format(
            int scopeLevel,
            string name,
            LogLevel logLevel,
            EventId eventId,
            string message,
            Exception exception)
        {
            const string Format = "{0}{2} [{3}]: {4}";
            var padding = new string(' ', scopeLevel * PaddingSpaces);

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