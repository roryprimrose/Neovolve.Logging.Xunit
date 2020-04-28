namespace Divergic.Logging.Xunit.UnitTests
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    public class LogLevelDataSet : IEnumerable<object[]>
    {
        private readonly IList<object[]> _data = new List<object[]>
        {
            new object[]
            {
                LogLevel.None, LogLevel.None, false
            },
            new object[]
            {
                LogLevel.None, LogLevel.Trace, false
            },
            new object[]
            {
                LogLevel.None, LogLevel.Debug, false
            },
            new object[]
            {
                LogLevel.None, LogLevel.Information, false
            },
            new object[]
            {
                LogLevel.None, LogLevel.Warning, false
            },
            new object[]
            {
                LogLevel.None, LogLevel.Error, false
            },
            new object[]
            {
                LogLevel.None, LogLevel.Critical, false
            },
            new object[]
            {
                LogLevel.Trace, LogLevel.None, false
            },
            new object[]
            {
                LogLevel.Trace, LogLevel.Trace, true
            },
            new object[]
            {
                LogLevel.Trace, LogLevel.Debug, true
            },
            new object[]
            {
                LogLevel.Trace, LogLevel.Information, true
            },
            new object[]
            {
                LogLevel.Trace, LogLevel.Warning, true
            },
            new object[]
            {
                LogLevel.Trace, LogLevel.Error, true
            },
            new object[]
            {
                LogLevel.Trace, LogLevel.Critical, true
            },
            new object[]
            {
                LogLevel.Debug, LogLevel.None, false
            },
            new object[]
            {
                LogLevel.Debug, LogLevel.Trace, false
            },
            new object[]
            {
                LogLevel.Debug, LogLevel.Debug, true
            },
            new object[]
            {
                LogLevel.Debug, LogLevel.Information, true
            },
            new object[]
            {
                LogLevel.Debug, LogLevel.Warning, true
            },
            new object[]
            {
                LogLevel.Debug, LogLevel.Error, true
            },
            new object[]
            {
                LogLevel.Debug, LogLevel.Critical, true
            },
            new object[]
            {
                LogLevel.Information, LogLevel.None, false
            },
            new object[]
            {
                LogLevel.Information, LogLevel.Trace, false
            },
            new object[]
            {
                LogLevel.Information, LogLevel.Debug, false
            },
            new object[]
            {
                LogLevel.Information, LogLevel.Information, true
            },
            new object[]
            {
                LogLevel.Information, LogLevel.Warning, true
            },
            new object[]
            {
                LogLevel.Information, LogLevel.Error, true
            },
            new object[]
            {
                LogLevel.Information, LogLevel.Critical, true
            },
            new object[]
            {
                LogLevel.Warning, LogLevel.None, false
            },
            new object[]
            {
                LogLevel.Warning, LogLevel.Trace, false
            },
            new object[]
            {
                LogLevel.Warning, LogLevel.Debug, false
            },
            new object[]
            {
                LogLevel.Warning, LogLevel.Information, false
            },
            new object[]
            {
                LogLevel.Warning, LogLevel.Warning, true
            },
            new object[]
            {
                LogLevel.Warning, LogLevel.Error, true
            },
            new object[]
            {
                LogLevel.Warning, LogLevel.Critical, true
            },
            new object[]
            {
                LogLevel.Error, LogLevel.None, false
            },
            new object[]
            {
                LogLevel.Error, LogLevel.Trace, false
            },
            new object[]
            {
                LogLevel.Error, LogLevel.Debug, false
            },
            new object[]
            {
                LogLevel.Error, LogLevel.Information, false
            },
            new object[]
            {
                LogLevel.Error, LogLevel.Warning, false
            },
            new object[]
            {
                LogLevel.Error, LogLevel.Error, true
            },
            new object[]
            {
                LogLevel.Error, LogLevel.Critical, true
            },
            new object[]
            {
                LogLevel.Critical, LogLevel.None, false
            },
            new object[]
            {
                LogLevel.Critical, LogLevel.Trace, false
            },
            new object[]
            {
                LogLevel.Critical, LogLevel.Debug, false
            },
            new object[]
            {
                LogLevel.Critical, LogLevel.Information, false
            },
            new object[]
            {
                LogLevel.Critical, LogLevel.Warning, false
            },
            new object[]
            {
                LogLevel.Critical, LogLevel.Error, false
            },
            new object[]
            {
                LogLevel.Critical, LogLevel.Critical, true
            }
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}