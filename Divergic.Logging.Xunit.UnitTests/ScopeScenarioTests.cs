namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;

    public class ScopeScenarioTests : LoggingTestsBase<ScopeScenarioTests>
    {
        public ScopeScenarioTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void TestOutputWritesMessagesInContextOfScopes()
        {
            Logger.LogCritical("Writing critical message");
            Logger.LogDebug("Writing debug message");
            Logger.LogError("Writing error message");
            Logger.LogInformation("Writing information message");
            Logger.LogTrace("Writing trace message");
            Logger.LogWarning("Writing warning message");

            using (Logger.BeginScope("First scope"))
            {
                Logger.LogInformation("Inside first scope");

                using (Logger.BeginScope("Second scope"))
                {
                    Logger.LogInformation("Inside second scope");
                }

                Logger.LogInformation("After second scope");
            }

            Logger.LogInformation("After first scope");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void TestOutputWritesMessagesUsingScopesWithoutState(string scopeState)
        {
            Logger.LogCritical("Writing critical message");
            Logger.LogDebug("Writing debug message");
            Logger.LogError("Writing error message");
            Logger.LogInformation("Writing information message");
            Logger.LogTrace("Writing trace message");
            Logger.LogWarning("Writing warning message");

            using (Logger.BeginScope((object) scopeState))
            {
                Logger.LogInformation("Inside first scope");

                using (Logger.BeginScope((object) scopeState))
                {
                    Logger.LogInformation("Inside second scope");
                }

                Logger.LogInformation("After second scope");
            }

            Logger.LogInformation("After first scope");
        }

        [Fact]
        public void TestOutputWritesScopeBoundariesUsingObjects()
        {
            Logger.LogCritical("Writing critical message");
            Logger.LogDebug("Writing debug message");
            Logger.LogError("Writing error message");
            Logger.LogInformation("Writing information message");
            Logger.LogTrace("Writing trace message");
            Logger.LogWarning("Writing warning message");

            var firstPerson = Model.Create<Person>();

            using (Logger.BeginScope(firstPerson))
            {
                Logger.LogInformation("Inside first scope");

                var secondPerson = Model.Create<Person>();

                using (Logger.BeginScope(secondPerson))
                {
                    Logger.LogInformation("Inside second scope");
                }

                Logger.LogInformation("After second scope");
            }

            Logger.LogInformation("After first scope");
        }

        [Fact]
        public void TestOutputWritesScopeBoundariesUsingValueTypes()
        {
            Logger.LogCritical("Writing critical message");
            Logger.LogDebug("Writing debug message");
            Logger.LogError("Writing error message");
            Logger.LogInformation("Writing information message");
            Logger.LogTrace("Writing trace message");
            Logger.LogWarning("Writing warning message");

            using (Logger.BeginScope(Guid.NewGuid()))
            {
                Logger.LogInformation("Inside first scope");

                using (Logger.BeginScope(Environment.TickCount))
                {
                    Logger.LogInformation("Inside second scope");
                }

                Logger.LogInformation("After second scope");
            }

            Logger.LogInformation("After first scope");
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        // ReSharper disable UnusedMember.Local
        private class Person
        {
            public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;

            public string Email { get; set; } = string.Empty;

            public string FirstName { get; set; } = string.Empty;

            public string LastName { get; set; } = string.Empty;
        }

        // ReSharper restore UnusedMember.Local
    }
}