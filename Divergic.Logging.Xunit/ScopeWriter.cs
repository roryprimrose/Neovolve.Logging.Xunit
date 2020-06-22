namespace Divergic.Logging.Xunit
{
    using System;
    using System.Globalization;
    using global::Xunit.Abstractions;
    using System.Text.Json;

    internal class ScopeWriter : IDisposable
    {
        private readonly LoggingConfig _config;
        private readonly int _depth;
        private readonly Action _onScopeEnd;
        private readonly ITestOutputHelper _outputHelper;
        private readonly object _state;
        private string _scopeMessage;
        private string _structuredStateData;

        public ScopeWriter(
            ITestOutputHelper outputHelper,
            object state,
            int depth,
            Action onScopeEnd,
            LoggingConfig config)
        {
            _outputHelper = outputHelper;
            _state = state;
            _depth = depth;
            _onScopeEnd = onScopeEnd;
            _config = config;

            DetermineScopeStateMessage();

            var scopeStartMessage = BuildScopeStateMessage(false);

            _outputHelper.WriteLine(scopeStartMessage);

            if (string.IsNullOrWhiteSpace(_structuredStateData) == false)
            {
                var padding = BuildPadding(_depth + 1);

                // Add the padding to the structured data
                var structuredLines =
                    _structuredStateData.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

                var structuredData = padding + string.Join(Environment.NewLine + padding, structuredLines);

                _outputHelper.WriteLine("{0}{1}{2}{3}", padding, "Scope data: ", Environment.NewLine, structuredData);
            }
        }

        public void Dispose()
        {
            var scopeStartMessage = BuildScopeStateMessage(true);

            _outputHelper.WriteLine(scopeStartMessage);

            _onScopeEnd?.Invoke();
        }

        private string BuildPadding(int depth)
        {
            return new string(' ', depth * _config.ScopePaddingSpaces);
        }

        private string BuildScopeStateMessage(bool isScopeEnd)
        {
            var padding = BuildPadding(_depth);
            var endScopeMarker = isScopeEnd ? "/" : string.Empty;
            const string format = "{0}<{1}{2}>";

            return string.Format(CultureInfo.InvariantCulture, format, padding, endScopeMarker, _scopeMessage);
        }

        private void DetermineScopeStateMessage()
        {
            const string scopeMarker = "Scope: ";
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
                    _scopeMessage = scopeMarker + state;
                }
            }
            else if (_state.GetType().IsValueType)
            {
                _scopeMessage = scopeMarker + _state;
            }
            else
            {
                try
                {
                    // The data is probably a complex object or a structured log entry
                    _structuredStateData = JsonSerializer.Serialize(_state, SerializerSettings.Default);
                }
                catch (JsonException ex)
                {
                    _structuredStateData = ex.ToString();
                }

                _scopeMessage = defaultScopeMessage;
            }
        }
    }
}