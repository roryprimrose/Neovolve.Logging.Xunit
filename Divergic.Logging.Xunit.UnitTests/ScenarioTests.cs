using System;
using System.Collections.Generic;
using System.Text;

namespace Divergic.Logging.Xunit.UnitTests
{
    using System.Linq;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    public class ScenarioTests
    {
        private readonly ITestOutputHelper _output;

        public ScenarioTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void LogEntryContainsSnapshotOfActiveScopes()
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
    }
}
