namespace Divergic.Logging.Xunit
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using EnsureThat;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="TestOutputLogger" />
    ///     class is used to provide logging implementation for Xunit.
    /// </summary>
    public class TestOutputLogger : FilterLogger
    {
        /// <summary>
        /// Identifies the number of spaces to use for indenting scopes.
        /// </summary>
        public const int PaddingSpaces = 3;
        private readonly string _name;
        private readonly ITestOutputHelper _output;
        private readonly Func<int, string, LogLevel, EventId, string, Exception, string> _customFormatter;
        private readonly Stack<ScopeWriter> _scopes;

        /// <summary>
        ///     Creates a new instance of the <see cref="TestOutputLogger" /> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="output">The test output helper.</param>
        /// <param name="customFormatter">Optional custom message formatter.</param>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c>, empty or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public TestOutputLogger(string name, ITestOutputHelper output, Func<int, string, LogLevel, EventId, string, Exception, string> customFormatter = null)
        {
            Ensure.String.IsNotNullOrWhiteSpace(name, nameof(name));
            Ensure.Any.IsNotNull(output, nameof(output));

            _name = name;
            _output = output;
            _customFormatter = customFormatter;
            _scopes = new Stack<ScopeWriter>();
        }

        /// <inheritdoc />
        public override IDisposable BeginScope<TState>(TState state)
        {
            var scopeWriter = new ScopeWriter(_output, state, _scopes.Count, () => _scopes.Pop());

            _scopes.Push(scopeWriter);

            return scopeWriter;
        }

        /// <inheritdoc />
        public override bool IsEnabled(LogLevel logLevel)
        {
            return true;
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
            if (_customFormatter != null)
            {
                var str = _customFormatter(_scopes.Count, _name, logLevel, eventId, message, exception);
                _output.WriteLine(str);
                return;
            }

            const string Format = "{0}{2} [{3}]: {4}";
            var padding = new string(' ', _scopes.Count * PaddingSpaces);

            if (string.IsNullOrWhiteSpace(message) == false)
            {
                _output.WriteLine(Format, padding, _name, logLevel, eventId.Id, message);
            }

            if (exception != null)
            {
                _output.WriteLine(Format, padding, _name, logLevel, eventId.Id, exception);
            }
        }

     }
}