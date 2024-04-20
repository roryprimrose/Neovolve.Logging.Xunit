﻿namespace Neovolve.Logging.Xunit
{
    using System;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The <see cref="LoggingTestsBase{T}" />
    ///     class is used to provide a simple logging bootstrap class for xUnit test classes.
    /// </summary>
    public abstract class LoggingTestsBase<T> : IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingTestsBase{T}" /> class.
        /// </summary>
        /// <param name="output">The xUnit test output.</param>
        /// <param name="logLevel">The minimum log level to output.</param>
        protected LoggingTestsBase(ITestOutputHelper output, LogLevel logLevel)
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
            Logger = output.BuildLoggerFor<T>(logLevel);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoggingTestsBase{T}" /> class.
        /// </summary>
        /// <param name="output">The xUnit test output.</param>
        /// <param name="config">Optional logging configuration.</param>
        protected LoggingTestsBase(ITestOutputHelper output, LoggingConfig? config = null)
        {
            Output = output ?? throw new ArgumentNullException(nameof(output));
            Logger = output.BuildLoggerFor<T>(config);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Disposes resources held by this instance.
        /// </summary>
        /// <param name="disposing"><c>true</c> if disposing unmanaged types; otherwise <c>false</c>.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Logger.Dispose();
            }
        }

        /// <summary>
        ///     Gets the logger instance.
        /// </summary>
        protected ICacheLogger<T> Logger { get; }

        /// <summary>
        ///     Gets the xUnit test output.
        /// </summary>
        protected ITestOutputHelper Output { get; }
    }
}