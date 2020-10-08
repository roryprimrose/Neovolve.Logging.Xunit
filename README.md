# Introduction

Divergic.Logging.Xunit is a NuGet package that returns an ```ILogger``` or ```ILogger<T>``` that wraps around the ```ITestOutputHelper``` supplied by xUnit. xUnit uses this helper to write log messages to the test output of each test execution. This means that any log messages from classes being tested will end up in the xUnit test result output.

[![GitHub license](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/Divergic/Divergic.Logging.Xunit/blob/master/LICENSE)&nbsp;[![Nuget](https://img.shields.io/nuget/v/Divergic.Logging.Xunit.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/Divergic.Logging.Xunit.svg)](https://www.nuget.org/packages/Divergic.Logging.Xunit)&nbsp;[![Actions Status](https://github.com/Divergic/Divergic.Logging.Xunit/workflows/CI/badge.svg)](https://github.com/Divergic/Divergic.Logging.Xunit/actions)

## Contents

[Installation][1]  
[Usage][2]  
[Output Formatting][3]  
[Inspection][4]  
[Configured LoggerFactory][5]  
[Existing Loggers][6]  
[Configuration][7]  
[Supporters][8]  

# Installation

Run the following in the NuGet command line or visit the [NuGet package page](https://nuget.org/packages/Divergic.Logging.Xunit).

```Install-Package Divergic.Logging.Xunit```

[Back to top][0]

# Usage

The common usage of this package is to call the `BuildLogger` extension method on the xUnit ```ITestOutputHelper```.

Consider the following example of a class to test.

```csharp
using System;
using Microsoft.Extensions.Logging;

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
```

Call `BuildLogger` on `ITestOutputHelper` to generate the `ILogger` that we can inject into the class being tested.

```csharp
using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

public class MyClassTests
{
    private readonly ITestOutputHelper _output;

    public MyClassTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void DoSomethingReturnsValue()
    {
        using var logger = _output.BuildLogger();

        var sut = new MyClass(logger);

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
public class MyClassTests
{
    private readonly ITestOutputHelper _output;

    public MyClassTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void DoSomethingReturnsValue()
    {
        using var logger = output.BuildLoggerFor<MyClass>();

        var sut = new MyClass(logger);

        var actual = sut.DoSomething();

        // The xUnit test output should now include the log message from MyClass.DoSomething()

        actual.Should().NotBeNull();
    }
}
```

The above examples inline the declaration of the logger with `using var` to ensure that the logger instance (and internal `ILoggerFactory`) is disposed. 

You can avoid having to build the logger instance in each unit test method by deriving the test class from either `LoggingTestsBase` or `LoggingTestsBase<T>`. These classes provide the implementation to build the logger and dispose it. They also provide access to the `ITestOutputHelper` instance for writing directly to the test output.

```csharp
public class MyClassTests : LoggingTestsBase
{
    public MyClassTests(ITestOutputHelper output) : base(output, LogLevel.Information)
    {
    }

    [Fact]
    public void DoSomethingReturnsValue()
    {
        var sut = new MyClass(Logger);

        var actual = sut.DoSomething();

        // The xUnit test output should now include the log message from
        MyClass.DoSomething()

        Output.WriteLine("This works too");

        actual.Should().NotBeNullOrWhiteSpace();
    }
}
```

The `BuildLogger` and `BuildLoggerFor<T>` extension methods along with the `LoggingTestsBase` and `LoggingTestsBase<T>` abstract classes also provide overloads to set the logging level or define 
[logging configuration][7].

[Back to top][0]

# Output Formatting

The default formatting to the xUnit test results may not be what you want. You can define your ```ILogFormatter``` class to control how the output looks.

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
    public MyConfig()
    {
        base.Formatter = new MyFormatter();
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

    public MyClassTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void DoSomethingReturnsValue()
    {
        using var logger = _output.BuildLogger(MyConfig.Current);
        var sut = new MyClass(logger);

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

    public MyClassTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void DoSomethingReturnsValue()
    {
        using var logger = _output.BuildLogger();
        var sut = new MyClass(logger);

        sut.DoSomething();
        
        logger.Count.Should().Be(1);
        logger.Entries.Should().HaveCount(1);
        logger.Last.Message.Should().Be("Hey, we did something");
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
    public void DoSomethingReturnsValue()
    {
        var logger = new CacheLogger();

        var sut = new MyClass(logger);

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
    public void DoSomethingReturnsValue()
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
    public void DoSomethingReturnsValue()
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
    public void DoSomethingReturnsValue()
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

**LogLevel**: Defines the minimum log level that will be written to the test output. This helps to limit the noise in test output when set to higher levels. Defaults to `LogLevel.Trace`.

**ScopePaddingSpaces**: Defines the number of spaces to use for indenting scopes.

[Back to top][0]

## Supporters

This project is supported by [JetBrains](https://www.jetbrains.com/?from=ModelBuilder)

[Back to top][0]

[0]: #introduction
[1]: #installation
[2]: #usage
[3]: #output-formatting
[4]: #inspection
[5]: #configured-loggerfactory
[6]: #existing-loggers
[7]: #configuration
[8]: #supporters
