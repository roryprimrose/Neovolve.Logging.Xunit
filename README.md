# Introduction

Divergic.Logging.Xunit is a NuGet package that returns an ```ILogger``` or ```ILogger<T>``` that wraps around the ```ITestOutputHelper``` supplied by xUnit. xUnit uses this helper to write log messages to the test output of each test execution. This means that any log messages from classes being tested will end up in the xUnit test result output.

## Contents

[Installation][1]  
[Usage][2]  
[Output Formatting][3]  
[Inspection][4]  
[Configured LoggerFactory][5]  
[Existing Loggers][6]  
[Configuration][7]  

# Installation

Run the following in the NuGet command line or visit the [NuGet package page](https://nuget.org/packages/Divergic.Logging.Xunit).

```Install-Package Divergic.Logging.Xunit```

[Back to top][0]

# Usage

The common usage of this package is to call the ```BuildLogger``` extension method on the xUnit ```ITestOutputHelper```.

```csharp
using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

public class MyClass
{
    private readonly ILogger _logger;

    public MyClass(ILogger logger)
    {
        _logger = logger;
    }

    public string DoSomething()
    {
        _logger.LogInformation("Hey, we did something");

        return Guid.NewGuid().ToString();
    }
}

public class MyClassTests
{
    private readonly ITestOutputHelper _output;
    private readonly ILogger _logger;

    public MyClassTests(ITestOutputHelper output)
    {
        _output = output;
        _logger = output.BuildLogger();
    }
    }

    [Fact]
    public void DoSomethingReturnsValueTest()
    {
        var sut = new MyClass(_logger);

        var actual = sut.DoSomething();

        // The xUnit test output should now include the log message from MyClass.DoSomething()

        actual.Should().NotBeNullOrWhiteSpace();
    }
}
```

This would output the following in the test results.

```
Information [0]: Hey, we did something
```

Support for ```ILogger<T>``` is there using the ```BuildLoggerFor<T>``` extension method.

```csharp
using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

public class MyClassTests
{
    private readonly ITestOutputHelper _output;
    private readonly ILogger<MyClass> _logger;

    public MyClassTests(ITestOutputHelper output)
    {
        _output = output;
        _logger = output.BuildLoggerFor<MyClass>();
    }

    [Fact]
    public void DoSomethingReturnsValueTest()
    {
        var sut = new MyClass(_logger);

        var actual = sut.DoSomething();

        // The xUnit test output should now include the log message from MyClass.DoSomething()

        actual.Should().NotBeNull();
    }
}
```

[Back to top][0]

# Output Formatting

The default formatting to the xUnit test results may not be what you want. You can now define your ```ILogFormatter``` class to control how the output looks.

```csharp
public class MyFormatter : ILogFormatter
{
    public string Format(
        int scopeLevel,
        string name,
        LogLevel logLevel,
        EventId eventId,
        string message,
        Exception exception)
    {
        var builder = new StringBuilder();

        if (scopeLevel > 0)
        {
            builder.Append(' ', scopeLevel * 2);
        }

        builder.Append($"{logLevel} ");

        if (!string.IsNullOrEmpty(name))
        {
            builder.Append($"{name} ");
        }

        if (eventId.Id != 0)
        {
            builder.Append($"[{eventId.Id}]: ");
        }

        if (!string.IsNullOrEmpty(message))
        {
            builder.Append(message);
        }

        if (exception != null)
        {
            builder.Append($"\n{exception}");
        }

        return builder.ToString();
    }
}

public class MyConfig : LoggingConfig
{
    public MyConfig() : base(new MyFormatter())
    {
    }

    public static MyConfig Current { get; } = new MyConfig();
}
```

The custom ```ILogFormatter``` is defined on a ```LoggingConfig``` class that can be provided when creating a logger. The ```MyConfig.Current``` property above is there provide a clean way to share the config across test classes.

```csharp
using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

public class MyClassTests
{
    private readonly ITestOutputHelper _output;
    private readonly ILogger _logger;

    public MyClassTests(ITestOutputHelper output)
    {
        _output = output;
        _logger = _output.BuildLogger(MyConfig.Current);
    }

    [Fact]
    public void DoSomethingReturnsValueTest()
    {
        var sut = new MyClass(_logger);

        var actual = sut.DoSomething();

        // The xUnit test output should now include the log message from MyClass.DoSomething()

        actual.Should().NotBeNull();
    }
}
```

[Back to top][0]

# Inspection

Using this library makes it really easy to output log messages from your code as part of the test results. You may want to also inspect the log messages written as part of the test assertions as well. 

The ```BuildLogger``` and ```BuildLoggerFor<T>``` extension methods support this by returning a ```ICacheLogger``` or ```ICacheLogger<T>``` respectively. The cache logger is a wrapper around the created logger and exposes all the log entries written by the test.

```csharp
using System;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

public class MyClassTests
{
    private readonly ITestOutputHelper _output;
    private readonly ICacheLogger _logger;

    public MyClassTests(ITestOutputHelper output)
    {
        _output = output;
        _logger = output.BuildLogger();
    }

    [Fact]
    public void DoSomethingReturnsValueTest()
    {
        var sut = new MyClass(_logger);

        sut.DoSomething();
        
        _logger.Count.Should().Be(1);
        _logger.Entries.Should().HaveCount(1);
        _logger.Last.Message.Should().Be("Hey, we did something");
    }
}
```

Perhaps you don't want to use the xUnit ```ITestOutputHelper``` but still want to use the ```ICacheLogger``` to run assertions over log messages written by the class under test. You can do this by creating a ```CacheLogger``` or ```CacheLogger<T>``` directly.

```csharp
using System;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

public class MyClassTests
{
    [Fact]
    public void DoSomethingReturnsValueTest()
    {
        var logger = new CacheLogger();

        var sut = new MyClass(_logger);

        sut.DoSomething();
        
        logger.Count.Should().Be(1);
        logger.Entries.Should().HaveCount(1);
        logger.Last.Message.Should().Be("Hey, we did something");
    }
}
```

[Back to top][0]

# Configured LoggerFactory

You may have an integration or acceptance test that requires additional configuration to the log providers on ```ILoggerFactory``` while also supporting the logging out to xUnit test results. You can do this by create a factory that is already configured with xUnit support.

```csharp
using System;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

public class MyClassTests
{
    private readonly ILogger _logger;

    public MyClassTests(ITestOutputHelper output)
    {
        var factory = LogFactory.Create(output);

        // call factory.AddConsole or other provider extension method

        _logger = factory.CreateLogger(nameof(MyClassTests));
    }

    [Fact]
    public void DoSomethingReturnsValueTest()
    {
        var sut = new MyClass(_logger);

        // The xUnit test output should now include the log message from MyClass.DoSomething()

        var actual = sut.DoSomething();

        actual.Should().NotBeNullOrWhiteSpace();
    }
}
```

[Back to top][0]

# Existing Loggers

Already have an existing logger and want the above cache support? Got you covered there too using the ```WithCache()``` method.

```csharp
using System;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

public class MyClassTests
{
    [Fact]
    public void DoSomethingReturnsValueTest()
    {
        var logger = Substitute.For<ILogger>();

        logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);

        var cacheLogger = logger.WithCache();

        var sut = new MyClass(cacheLogger);

        sut.DoSomething();

        cacheLogger.Count.Should().Be(1);
        cacheLogger.Entries.Should().HaveCount(1);
        cacheLogger.Last.Message.Should().Be("Hey, we did something");
    }
}
```

The ```WithCache()``` also supports ```ILogger<T>```.

```csharp
using System;
using Divergic.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

public class MyClassTests
{
    [Fact]
    public void DoSomethingReturnsValueTest()
    {
        var logger = Substitute.For<ILogger<MyClass>>();

        logger.IsEnabled(Arg.Any<LogLevel>()).Returns(true);

        var cacheLogger = logger.WithCache();

        var sut = new MyClass(cacheLogger);

        sut.DoSomething();

        cacheLogger.Count.Should().Be(1);
        cacheLogger.Entries.Should().HaveCount(1);
        cacheLogger.Last.Message.Should().Be("Hey, we did something");
    }
}
```

[Back to top][0]

# Configuration

Logging configuration can be controled by using a ```LoggingConfig``` class as indicated in the [Output Formatting][3] section above. The following are the configuration options that can be set.

**Formatter**: Defines a custom formatting for rendering log messages to xUnit test output.

**IgnoreTestBoundaryException**: Defines whether exceptions thrown while logging outside of the test execution will be ignored.

**ScopePaddingSpaces**: Defines the number of spaces to use for indenting scopes.

[Back to top][0]

[0]: #introduction
[1]: #installation
[2]: #usage
[3]: #output-formatting
[4]: #inspection
[5]: #configured-loggerfactory
[6]: #existing-loggers
[7]: #configuration