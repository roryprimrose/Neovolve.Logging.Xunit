namespace Divergic.Logging.Xunit
{
    using System;

    internal class CacheScope : IDisposable
    {
        private Action _onScopeEnd;
        private IDisposable _scope;

        public CacheScope(IDisposable scope, object state, Action onScopeEnd)
        {
            _scope = scope;
            State = state;
            _onScopeEnd = onScopeEnd;
        }

        public void Dispose()
        {
            // Pass on the end scope request
            _scope?.Dispose();
            _scope = null;

            // Clean up the scope in the cache logger
            _onScopeEnd?.Invoke();
            _onScopeEnd = null;
        }

        public object State { get; }
    }
}