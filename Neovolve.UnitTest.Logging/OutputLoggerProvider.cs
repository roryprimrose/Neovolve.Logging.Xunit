namespace Neovolve.UnitTest.Logging
{
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    /// <summary>
    ///     The <see cref="OutputLoggerProvider" />
    ///     class is used to provide Xunit logging to <see cref="ILoggerFactory" />.
    /// </summary>
    public class OutputLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _output;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OutputLoggerProvider" /> class.
        /// </summary>
        /// <param name="output">The test output helper.</param>
        public OutputLoggerProvider(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // no-op
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName)
        {
            return new OutputLogger(categoryName, _output);
        }
    }
}