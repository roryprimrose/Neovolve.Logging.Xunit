namespace Divergic.Logging.Xunit
{
    using System;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="LoggingConfig" />
    ///     class is used to configure how logging operates.
    /// </summary>
    public class LoggingConfig
    {
        private ILogFormatter _formatter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingConfig" /> class.
        /// </summary>
        public LoggingConfig()
        {
            _formatter = new DefaultFormatter(this);
        }

        /// <summary>
        ///     Gets or sets a custom formatting for rendering log messages to xUnit test output.
        /// </summary>
        public ILogFormatter Formatter
        {
            get => _formatter;
            set =>
                _formatter = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        ///     Gets or sets whether exceptions thrown while logging outside of the test execution will be ignored.
        /// </summary>
        public bool IgnoreTestBoundaryException { get; set; }

        /// <summary>
        ///     Gets or sets the minimum logging level.
        /// </summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        ///     Identifies the number of spaces to use for indenting scopes.
        /// </summary>
        public int ScopePaddingSpaces { get; set; } = 3;
    }
}