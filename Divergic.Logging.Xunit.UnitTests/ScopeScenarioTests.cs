namespace Divergic.Logging.Xunit.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;
    using ModelBuilder;

    public class ScopeScenarioTests : LoggingTestsBase<ScopeScenarioTests>
    {
        private static readonly LoggingConfig _config = new LoggingConfig().Set(x => x.SensitiveValues.Add("secret"));

        public ScopeScenarioTests(ITestOutputHelper output) : base(output, _config)
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

        [Fact]
        public void TestOutputWritesMessagesInContextOfScopesWithSensitiveData()
        {
            Logger.LogCritical("Writing critical message with secret");
            Logger.LogDebug("Writing debug message with secret");
            Logger.LogError("Writing error message with secret");
            Logger.LogInformation("Writing information message with secret");
            Logger.LogTrace("Writing trace message with secret");
            Logger.LogWarning("Writing warning message with secret");

            using (Logger.BeginScope("First scope with secret"))
            {
                Logger.LogInformation("Inside first scope with secret");

                using (Logger.BeginScope("Second scope with secret"))
                {
                    Logger.LogInformation("Inside second scope with secret");
                }

                Logger.LogInformation("After second scope with secret");
            }

            Logger.LogInformation("After first scope with secret");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void TestOutputWritesMessagesUsingScopesWithoutState(string? scopeState)
        {
            Logger.LogCritical("Writing critical message");
            Logger.LogDebug("Writing debug message");
            Logger.LogError("Writing error message");
            Logger.LogInformation("Writing information message");
            Logger.LogTrace("Writing trace message");
            Logger.LogWarning("Writing warning message");

            using (Logger.BeginScope((object) scopeState!))
            {
                Logger.LogInformation("Inside first scope");

                using (Logger.BeginScope((object) scopeState!))
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

            var firstPerson = Model.Create<StructuredData>();

            using (Logger.BeginScope(firstPerson))
            {
                Logger.LogInformation("Inside first scope");

                var secondPerson = Model.Create<StructuredData>();

                using (Logger.BeginScope(secondPerson))
                {
                    Logger.LogInformation("Inside second scope");
                }

                Logger.LogInformation("After second scope");
            }

            Logger.LogInformation("After first scope");
        }

        [Fact]
        public void TestOutputWritesScopeBoundariesUsingObjectsWithSecret()
        {
            Logger.LogCritical("Writing critical message with secret");
            Logger.LogDebug("Writing debug message with secret");
            Logger.LogError("Writing error message with secret");
            Logger.LogInformation("Writing information message with secret");
            Logger.LogTrace("Writing trace message with secret");
            Logger.LogWarning("Writing warning message with secret");

            var firstPerson = Model.Create<StructuredData>().Set(x => x.Email = "secret");

            using (Logger.BeginScope(firstPerson))
            {
                Logger.LogInformation("Inside first scope with secret");

                var secondPerson = Model.Create<StructuredData>().Set(x => x.FirstName = "secret");

                using (Logger.BeginScope(secondPerson))
                {
                    Logger.LogInformation("Inside second scope with secret");
                }

                Logger.LogInformation("After second scope with secret");
            }

            Logger.LogInformation("After first scope with secret");
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

        [Fact]
        public async Task UsingParallelTasks()
        {
            var tasks = Enumerable.Range(0, 10).Select(
                _ => StartOnDefaultScheduler(
                    () =>
                    {
                        for (var i = 0; i < 100; i++)
                        {
                            using (Logger.BeginScope("My scope data"))
                            {
                            }
                        }

                        return Task.CompletedTask;
                    })).ToList();

            await Task.WhenAll(tasks);

            Task StartOnDefaultScheduler(Func<Task> asyncFunc)
            {
                return Task.Factory.StartNew(
                    asyncFunc,
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    TaskScheduler.Default).Unwrap();
            }
        }

        [Fact]
        public void UsingThreads()
        {
            var threads = Enumerable.Range(0, 10).Select(
                _ => new Thread(
                    () =>
                    {
                        for (var i = 0; i < 100; i++)
                        {
                            using (Logger.BeginScope("My scope data"))
                            {
                            }
                        }
                    })).ToList();

            threads.ForEach(x => x.Start());
            threads.ForEach(x => x.Join());
        }
    }
}