namespace Divergic.Logging.Xunit
{
    /// <summary>
    ///     The <see cref="LoggingConfig" />
    ///     class is used to configure how logging operates.
    /// </summary>
    public class LoggingConfig
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingConfig" /> class.
        /// </summary>
        public LoggingConfig()
        {
            Formatter = new DefaultFormatter(this);
        }

        /// <summary>
        ///     Gets or sets a custom formatting for rendering log messages to xUnit test output.
        /// </summary>
        public ILogFormatter Formatter { get; set; }

        /// <summary>
        ///     Gets or sets whether exceptions thrown while logging outside of the test execution will be ignored.
        /// </summary>
        public bool IgnoreTestBoundaryException { get; set; }

        /// <summary>
        ///     Identifies the number of spaces to use for indenting scopes.
        /// </summary>
        public int ScopePaddingSpaces { get; set; } = 3;
    }
}