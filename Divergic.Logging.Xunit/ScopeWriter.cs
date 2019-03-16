namespace Divergic.Logging.Xunit
{
    using System;
    using System.Globalization;
    using global::Xunit.Abstractions;
    using Newtonsoft.Json;

    internal class ScopeWriter : IDisposable
    {
        private readonly int _depth;
        private readonly Action _onScopeEnd;
        private readonly ITestOutputHelper _outputHelper;
        private readonly object _state;

        public ScopeWriter(ITestOutputHelper outputHelper, object state, int depth, Action onScopeEnd)
        {
            _outputHelper = outputHelper;
            _state = state;
            _depth = depth;
            _onScopeEnd = onScopeEnd;

            WriteScopeBoundary(false);
        }

        public void Dispose()
        {
            WriteScopeBoundary(true);

            _onScopeEnd?.Invoke();
        }

        private string BuildScopeStateMessage(bool isScopeEnd)
        {
            var padding = new string(' ', _depth * TestOutputLogger.PaddingSpaces);
            var endScopeMarker = isScopeEnd ? "/" : string.Empty;
            const string Format = "{0}<{1}{2}>";

            var message = DetermineScopeStateMessage();

            return string.Format(CultureInfo.InvariantCulture, Format, padding, endScopeMarker, message);
        }

        private string DetermineScopeStateMessage()
        {
            if (_state == null)
            {
                return "Scope " + (_depth + 1);
            }

            if (_state is string state)
            {
                return state;
            }

            if (_state.GetType().IsValueType)
            {
                return _state.ToString();
            }

            var data = JsonConvert.SerializeObject(_state);

            return data;
        }

        private void WriteScopeBoundary(bool isScopeEnd)
        {
            var message = BuildScopeStateMessage(isScopeEnd);

            _outputHelper.WriteLine(message);
        }
    }
}