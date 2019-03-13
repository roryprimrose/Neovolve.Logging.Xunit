namespace Divergic.Logging.Xunit
{
    using System;
    using System.Collections.Generic;
    using EnsureThat;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    ///     The <see cref="TestOutputLogger" />
    ///     class is used to provide logging implementation for Xunit.
    /// </summary>
    public class TestOutputLogger : FilterLogger, IDisposable
    {
        private readonly string _name;
        private readonly ITestOutputHelper _output;
        private readonly Stack<object> _scopes;

        /// <summary>
        ///     Creates a new instance of the <see cref="TestOutputLogger" /> class.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="output">The test output helper.</param>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c>, empty or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public TestOutputLogger(string name, ITestOutputHelper output)
        {
            Ensure.String.IsNotNullOrWhiteSpace(name, nameof(name));
            Ensure.Any.IsNotNull(output, nameof(output));

            _name = name;
            _output = output;
            _scopes = new Stack<object>();
        }

        /// <inheritdoc />
        public override IDisposable BeginScope<TState>(TState state)
        {
            _scopes.Push(state);

            return this;
        }

        /// <inheritdoc />
        public override bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        /// <inheritdoc />
        protected override void WriteLogEntry<TState>(LogLevel logLevel, EventId eventId, TState state, string message,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            const string format = "{1} [{2}]: {3}";

            if (string.IsNullOrWhiteSpace(message) == false)
            {
                _output.WriteLine(format, _name, logLevel, eventId.Id, message);
            }

            if (exception != null)
            {
                _output.WriteLine(format, _name, logLevel, eventId.Id, exception);
            }

            if (_scopes.Count > 0)
            {
                _output.WriteLine(JsonConvert.SerializeObject(_scopes, Formatting.Indented));
            }
        }

        void IDisposable.Dispose()
        {
            if (_scopes.Count > 0)
            {
                _scopes.Pop();
            }
        }
    }
}