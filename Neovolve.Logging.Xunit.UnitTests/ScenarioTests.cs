namespace Neovolve.Logging.Xunit.UnitTests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using global::Xunit;
    using Microsoft.Extensions.Logging;

    public class ScenarioTests : LoggingTestsBase
    {
        public ScenarioTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void TestOutputWritesMessages()
        {
            var loggedEntries = new List<LogEntry>();

            Logger.LogWritten += (_, entry) =>
            {
                loggedEntries.Add(entry);
            };

            Logger.LogCritical("Writing critical message");
            Logger.LogDebug("Writing debug message");
            Logger.LogError("Writing error message");
            Logger.LogInformation("Writing information message");
            Logger.LogTrace("Writing trace message");
            Logger.LogWarning("Writing warning message");

            Output.WriteLine("All finished");

            loggedEntries.Should().HaveCount(6);
            loggedEntries[0].LogLevel.Should().Be(LogLevel.Critical);
            loggedEntries[0].Message.Should().Be("Writing critical message");
            loggedEntries[5].LogLevel.Should().Be(LogLevel.Warning);
            loggedEntries[5].Message.Should().Be("Writing warning message");
        }
    }
}