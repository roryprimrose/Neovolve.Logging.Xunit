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
        private string _scopeMessage;
        private string _structuredStateData;

        public ScopeWriter(ITestOutputHelper outputHelper, object state, int depth, Action onScopeEnd)
        {
            _outputHelper = outputHelper;
            _state = state;
            _depth = depth;
            _onScopeEnd = onScopeEnd;

            DetermineScopeStateMessage();

            var scopeStartMessage = BuildScopeStateMessage(false);

            _outputHelper.WriteLine(scopeStartMessage);

            if (string.IsNullOrWhiteSpace(_structuredStateData) == false)
            {
                var padding = BuildPadding(_depth + 1);

                _outputHelper.WriteLine(padding + "Scope data: " + _structuredStateData);
            }
        }

        public void Dispose()
        {
            var scopeStartMessage = BuildScopeStateMessage(true);

            _outputHelper.WriteLine(scopeStartMessage);

            _onScopeEnd?.Invoke();
        }

        private string BuildScopeStateMessage(bool isScopeEnd)
        {
            var padding = BuildPadding(_depth);
            var endScopeMarker = isScopeEnd ? "/" : string.Empty;
            const string Format = "{0}<{1}{2}>";

            return string.Format(CultureInfo.InvariantCulture, Format, padding, endScopeMarker, _scopeMessage);
        }

        private string BuildPadding(int depth)
        {
            return new string(' ', depth * TestOutputLogger.PaddingSpaces);
        }

        private void DetermineScopeStateMessage()
        {
            const string ScopeMarker = "Scope: ";
            var defaultScopeMessage = "Scope " + (_depth + 1);

            if (_state == null)
            {
                _scopeMessage = defaultScopeMessage;
            }
            else if (_state is string state)
            {
                if (string.IsNullOrWhiteSpace(state))
                {
                    _scopeMessage = defaultScopeMessage;
                }
                else
                {
                    _scopeMessage = ScopeMarker + state;
                }
            }
            else if (_state.GetType().IsValueType)
            {
                _scopeMessage = ScopeMarker + _state;
            }
            else
            {
                // The data is probably a complex object or a structured log entry
                _structuredStateData = JsonConvert.SerializeObject(_state);

                _scopeMessage = defaultScopeMessage;
            }
        }
    }
}