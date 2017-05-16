namespace Neovolve.UnitTest.Logging
{
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    public class OutputLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _output;

        public OutputLoggerProvider(ITestOutputHelper output)
        {
            _output = output;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new OutputLogger(categoryName, _output);
        }

        public void Dispose()
        {
            // no-op
        }
    }
}