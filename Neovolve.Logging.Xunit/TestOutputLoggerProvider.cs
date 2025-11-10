namespace Neovolve.Logging.Xunit
{
    using System;
    using System.Collections.Concurrent;
    using global::Xunit;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="TestOutputLoggerProvider" /> class is used to provide Xunit logging to <see cref="ILoggerFactory" />
    ///     .
    /// </summary>
    public sealed class TestOutputLoggerProvider : ILoggerProvider
    {
        private readonly LoggingConfig? _config;
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new();
        private readonly ITestOutputHelper _output;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TestOutputLoggerProvider" /> class.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        /// <param name="config">Optional logging configuration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public TestOutputLoggerProvider(ITestOutputHelper output, LoggingConfig? config = null)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
            _config = config;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">The <paramref name="categoryName" /> is <c>null</c>, empty or whitespace.</exception>
        public ILogger CreateLogger(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("No categoryName value has been supplied", nameof(categoryName));
            }

            return _loggers.GetOrAdd(categoryName, name => new TestOutputLogger(name, _output, _config));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}