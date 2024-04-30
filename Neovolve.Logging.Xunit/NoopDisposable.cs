﻿namespace Neovolve.Logging.Xunit
{
    using System;

    internal class NoopDisposable : IDisposable
    {
        public static readonly NoopDisposable Instance = new();

        public void Dispose()
        {
            // No-op
        }
    }
}