namespace Neovolve.Logging.Xunit.UnitTests
{
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    public class ScenarioTests : LoggingTestsBase
    {
        public ScenarioTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void TestOutputWritesMessages()
        {
            Logger.LogCritical("Writing critical message");
            Logger.LogDebug("Writing debug message");
            Logger.LogError("Writing error message");
            Logger.LogInformation("Writing information message");
            Logger.LogTrace("Writing trace message");
            Logger.LogWarning("Writing warning message");

            Output.WriteLine("All finished");
        }
    }
}