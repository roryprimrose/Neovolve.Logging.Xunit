/*
[4/16/2019 4:41:19 PM Error] The active test run was aborted. 
Reason: Unhandled Exception: System.AggregateException: 
An error occurred while writing to logger(s). (There is no currently active test.) 
---> System.InvalidOperationException: There is no currently active test.
   at Xunit.Sdk.TestOutputHelper.GuardInitialized() in C:\Dev\xunit\xunit\src\xunit.execution\Sdk\Frameworks\TestOutputHelper.cs:line 51
   at Xunit.Sdk.TestOutputHelper.QueueTestOutput(String output) in C:\Dev\xunit\xunit\src\xunit.execution\Sdk\Frameworks\TestOutputHelper.cs:line 62
   at Divergic.Logging.Xunit.TestOutputLogger.WriteLogEntry[TState](LogLevel logLevel, EventId eventId, TState state, String message, Exception exception, Func`3 formatter) in D:\a\1\s\Divergic.Logging.Xunit\TestOutputLogger.cs:line 91
   at Divergic.Logging.Xunit.FilterLogger.Log[TState](LogLevel logLevel, EventId eventId, TState state, Exception exception, Func`3 formatter) in D:\a\1\s\Divergic.Logging.Xunit\FilterLogger.cs:line 46
   at Microsoft.Extensions.Logging.Logger.Log[TState](LogLevel logLevel, EventId eventId, TState state, Exception exception, Func`3 formatter)
   --- End of inner exception stack trace ---
   at Microsoft.Extensions.Logging.Logger.Log[TState](LogLevel logLevel, EventId eventId, TState state, Exception exception, Func`3 formatter)
   at Microsoft.Extensions.Logging.Logger`1.Microsoft.Extensions.Logging.ILogger.Log[TState](LogLevel logLevel, EventId eventId, TState state, Exception exception, Func`3 formatter)
   at Microsoft.Extensions.Logging.LoggerExtensions.Log(ILogger logger, LogLevel logLevel, EventId eventId, Exception exception, String message, Object[] args)
   at Microsoft.Extensions.Logging.LoggerExtensions.LogInformation(ILogger logger, Exception exception, String message, Object[] args)
   (my code)
 */

namespace Divergic.Logging.Xunit.UnitTests
{
    using System.Threading.Tasks;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    public class TaskThrowTest
    {
        private readonly ILogger Logger;

        public TaskThrowTest(ITestOutputHelper output)
        {
            Logger = output.BuildLogger("name");
        }

        // If WriteLogEntry() is called after a test has completed, an InvalidOperationException may be thrown.
        // To guard against this, exceptions can be ignored.
        // Please see see TestOutputLogger.cs:74
        // There is probably a better solution.

        [Fact]
        public void TestWriteLogEntry()
        {
            var task = new Task(async () =>
            {
                await Task.Delay(0).ConfigureAwait(false);
                Logger.LogCritical("message2");
            });

            task.Start();
        }
    }
}