namespace Divergic.Logging.Xunit
{
    using System;
    using EnsureThat;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="TestOutputLoggerProvider" /> class is used to provide Xunit logging to <see cref="ILoggerFactory" />
    ///     .
    /// </summary>
    public class TestOutputLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TestOutputLoggerProvider" /> class.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="output" /> is <c>null</c>.</exception>
        public TestOutputLoggerProvider(ITestOutputHelper output)
        {
            Ensure.Any.IsNotNull(output, nameof(output));

            _output = output;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">The <paramref name="categoryName" /> is <c>null</c>, empty or whitespace.</exception>
        public ILogger CreateLogger(string categoryName)
        {
            Ensure.String.IsNotNullOrWhiteSpace(categoryName);

            return new TestOutputLogger(categoryName, _output);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // no-op
        }
    }
}