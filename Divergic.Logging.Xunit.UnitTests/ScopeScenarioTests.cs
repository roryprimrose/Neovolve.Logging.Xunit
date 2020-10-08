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

            using (Logger.BeginScope((object)scopeState))
            {
                Logger.LogInformation("Inside first scope");

                using (Logger.BeginScope((object)scopeState))
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

            await Task.WhenAll(tasks).ConfigureAwait(false);

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

        // ReSharper disable once ClassNeverInstantiated.Local
        // ReSharper disable UnusedMember.Local
        private class Person
        {
            public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;

            public string Email { get; set; } = string.Empty;

            public string FirstName { get; set; } = string.Empty;

            public string LastName { get; set; } = string.Empty;
        }
        // ReSharper restore once ClassNeverInstantiated.Local
        // ReSharper restore UnusedMember.Local
    }
}