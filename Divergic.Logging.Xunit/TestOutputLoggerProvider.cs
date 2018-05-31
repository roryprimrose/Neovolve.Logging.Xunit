namespace Divergic.Logging.Xunit
{
    using System;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="TestOutputLoggerProvider" /> class is used to provide Xunit logging to <see cref="ILoggerFactory" />.
    /// </summary>
    public class TestOutputLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TestOutputLoggerProvider" /> class.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        public TestOutputLoggerProvider(ITestOutputHelper output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return new TestOutputLogger(categoryName, _output);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // no-op
        }
    }
}