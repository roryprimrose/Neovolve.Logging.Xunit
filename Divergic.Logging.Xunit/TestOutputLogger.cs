namespace Divergic.Logging.Xunit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using EnsureThat;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="TestOutputLogger" />
    ///     class is used to provide logging implementation for Xunit.
    /// </summary>
    public class TestOutputLogger : FilterLogger
    {
        private readonly LoggingConfig _config;
        private readonly ILogFormatter _formatter;
        private readonly string _name;
        private readonly ITestOutputHelper _output;
        private readonly Stack<ScopeWriter> _scopes;

        /// <summary>
        ///     Creates a new instance of the <see cref="TestOutputLogger" /> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="output">The test output helper.</param>
        /// <param name="config">Optional logging configuration.</param>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c>, empty or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public TestOutputLogger(string name, ITestOutputHelper output, LoggingConfig config = null)
        {
            Ensure.String.IsNotNullOrWhiteSpace(name, nameof(name));
            Ensure.Any.IsNotNull(output, nameof(output));

            _name = name;
            _output = output;
            _config = config ?? new LoggingConfig();
            _formatter = _config.Formatter ?? new DefaultFormatter(_config);

            _scopes = new Stack<ScopeWriter>();
        }

        /// <inheritdoc />
        public override IDisposable BeginScope<TState>(TState state)
        {
            var scopeWriter = new ScopeWriter(_output, state, _scopes.Count, () => _scopes.Pop(), _config);

            _scopes.Push(scopeWriter);

            return scopeWriter;
        }

        /// <inheritdoc />
        public override bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.None)
            {
                return false;
            }

            return logLevel >= _config.LogLevel;
        }

        /// <inheritdoc />
        protected override void WriteLogEntry<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            string message,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            try
            {
                WriteLog(logLevel, eventId, message, exception);
            }
            catch (InvalidOperationException)
            {
                if (_config.IgnoreTestBoundaryException == false)
                {
                    throw;
                }
            }
        }

        private void WriteLog(LogLevel logLevel, EventId eventId, string message, Exception exception)
        {
            var formattedMessage = _formatter.Format(_scopes.Count, _name, logLevel, eventId, message, exception);

            _output.WriteLine(formattedMessage);
            
            // Write the message to the output window
            Trace.WriteLine(formattedMessage);
        }
    }
}