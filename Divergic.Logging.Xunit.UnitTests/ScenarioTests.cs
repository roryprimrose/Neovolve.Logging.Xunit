namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;

    public class ScenarioTests
    {
        private readonly ITestOutputHelper _output;

        public ScenarioTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestOutputWritesMessagesInContextOfScopes()
        {
            var logger = _output.BuildLogger();

            logger.LogCritical("Writing critical message");
            logger.LogDebug("Writing debug message");
            logger.LogError("Writing error message");
            logger.LogInformation("Writing information message");
            logger.LogTrace("Writing trace message");
            logger.LogWarning("Writing warning message");

            using (logger.BeginScope("First scope"))
            {
                logger.LogInformation("Inside first scope");

                using (logger.BeginScope("Second scope"))
                {
                    logger.LogInformation("Inside second scope");
                }

                logger.LogInformation("After second scope");
            }

            logger.LogInformation("After first scope");
        }

        [Fact]
        public void TestOutputWritesMessagesUsingScopesWithoutState()
        {
            var logger = _output.BuildLogger();

            logger.LogCritical("Writing critical message");
            logger.LogDebug("Writing debug message");
            logger.LogError("Writing error message");
            logger.LogInformation("Writing information message");
            logger.LogTrace("Writing trace message");
            logger.LogWarning("Writing warning message");

            using (logger.BeginScope((object) null))
            {
                logger.LogInformation("Inside first scope");

                using (logger.BeginScope((object) null))
                {
                    logger.LogInformation("Inside second scope");
                }

                logger.LogInformation("After second scope");
            }

            logger.LogInformation("After first scope");
        }

        [Fact]
        public void TestOutputWritesScopeBoundariesUsingObjects()
        {
            var logger = _output.BuildLogger();

            logger.LogCritical("Writing critical message");
            logger.LogDebug("Writing debug message");
            logger.LogError("Writing error message");
            logger.LogInformation("Writing information message");
            logger.LogTrace("Writing trace message");
            logger.LogWarning("Writing warning message");

            var firstPerson = Model.Create<Person>();

            using (logger.BeginScope(firstPerson))
            {
                logger.LogInformation("Inside first scope");

                var secondPerson = Model.Create<Person>();

                using (logger.BeginScope(secondPerson))
                {
                    logger.LogInformation("Inside second scope");
                }

                logger.LogInformation("After second scope");
            }

            logger.LogInformation("After first scope");
        }

        [Fact]
        public void TestOutputWritesScopeBoundariesUsingValueTypes()
        {
            var logger = _output.BuildLogger();

            logger.LogCritical("Writing critical message");
            logger.LogDebug("Writing debug message");
            logger.LogError("Writing error message");
            logger.LogInformation("Writing information message");
            logger.LogTrace("Writing trace message");
            logger.LogWarning("Writing warning message");

            using (logger.BeginScope(Guid.NewGuid()))
            {
                logger.LogInformation("Inside first scope");

                using (logger.BeginScope(Environment.TickCount))
                {
                    logger.LogInformation("Inside second scope");
                }

                logger.LogInformation("After second scope");
            }

            logger.LogInformation("After first scope");
        }

        private class Person
        {
            public DateTime DateOfBirth { get; set; }

            public string Email { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }
    }
}