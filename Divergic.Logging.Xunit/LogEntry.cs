namespace Divergic.Logging.Xunit
{
    using System;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="LogEntry" />
    ///     class is used to identify the data related to a log entry.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogEntry" /> class.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="eventId">The event id.</param>
        /// <param name="state">The state.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public LogEntry(LogLevel logLevel, EventId eventId, object state, Exception exception, string message)
        {
            LogLevel = logLevel;
            EventId = eventId;
            State = state;
            Exception = exception;
            Message = message;
        }

        /// <summary>
        ///     Gets the event id of the entry.
        /// </summary>
        public EventId EventId { get; }

        /// <summary>
        ///     Gets the exception of the entry.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        ///     Gets the log level of the entry.
        /// </summary>
        public LogLevel LogLevel { get; }

        /// <summary>
        ///     Gets the message of the entry.
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///     Gets the state of the entry.
        /// </summary>
        public object State { get; }
    }
}