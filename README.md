# Introduction

Neovolve.Logging.Xunit is a NuGet package that returns an `ILogger` or `ILogger<T>` that wraps around the `ITestOutputHelper` supplied by xUnit. xUnit uses this helper to write log messages to the test output of each test execution. This means that any log messages from classes being tested will end up in the xUnit test result output.

[![GitHub license](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/roryprimrose/Neovolve.Logging.Xunit/blob/master/LICENSE)&nbsp;[![Actions Status](https://github.com/roryprimrose/Neovolve.Logging.Xunit/workflows/CI/badge.svg)](https://github.com/roryprimrose/Neovolve.Logging.Xunit/actions)

Neovolve.Logging.Xunit.v3  
[![Nuget](https://img.shields.io/nuget/v/Neovolve.Logging.Xunit.v3.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/Neovolve.Logging.Xunit.v3.svg)](https://www.nuget.org/packages/Neovolve.Logging.Xunit.v3)

Neovolve.Logging.Xunit.Signed.v3  
[![Nuget](https://img.shields.io/nuget/v/Neovolve.Logging.Xunit.Signed.v3.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/Neovolve.Logging.Xunit.Signed.v3.svg)](https://www.nuget.org/packages/Neovolve.Logging.Xunit.v3.Signed)

Neovolve.Logging.Xunit  
[![Nuget](https://img.shields.io/nuget/v/Neovolve.Logging.Xunit.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/Neovolve.Logging.Xunit.svg)](https://www.nuget.org/packages/Neovolve.Logging.Xunit)

Neovolve.Logging.Xunit.Signed  
[![Nuget](https://img.shields.io/nuget/v/Neovolve.Logging.Xunit.Signed.svg)&nbsp;![Nuget](https://img.shields.io/nuget/dt/Neovolve.Logging.Xunit.Signed.svg)](https://www.nuget.org/packages/Neovolve.Logging.Xunit.Signed)

- [Installation](#installation)
  - [Using xUnit v3](#using-xunit-v3)
  - [Using xUnit v2](#using-xunit-v2)
- [Usage](#usage)
- [Output Formatting](#output-formatting)
- [Inspection](#inspection)
- [Configured LoggerFactory](#configured-loggerfactory)
- [Existing Loggers](#existing-loggers)
- [Sensitive Values](#sensitive-values)
- [Configuration](#configuration)

## Installation

### Using xUnit v3

Run the following in the NuGet command line or visit the [NuGet package page](https://nuget.org/packages/Neovolve.Logging.Xunit.v3).

`Install-Package Neovolve.Logging.Xunit.v3`

If you need a strong named version of this library, run the following in the NuGet command line or visit the [NuGet package page](https://nuget.org/packages/Neovolve.Logging.Xunit.Signed.v3).

`Install-Package Neovolve.Logging.Xunit.Signed.v3`

[Back to top][0]

### Using xUnit v2

Run the following in the NuGet command line or visit the [NuGet package page](https://nuget.org/packages/Neovolve.Logging.Xunit).

`Install-Package Neovolve.Logging.Xunit`

If you need a strong named version of this library, run the following in the NuGet command line or visit the [NuGet package page](https://nuget.org/packages/Neovolve.Logging.Xunit.Signed).

`Install-Package Neovolve.Logging.Xunit.Signed`

[Back to top][0]

## Usage

The common usage of this package is to call the `BuildLogger<T>` extension method on the xUnit `ITestOutputHelper`.

Consider the following example of a class to test.

```csharp
using System;
using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger _logger;

    public MyClass(ILogger<MyClass> logger)
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

Call `BuildLoggerFor<T>()` on `ITestOutputHelper` to generate the `ILogger<T>` to inject into the class being tested.

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

This would output the following in the test results.

```
Information [0]: Hey, we did something
```

Similarly, using the `BuildLogger()` extension method will return an `ILogger` configured with xUnit test output.

The above examples inline the declaration of the logger with `using var` to ensure that the logger instance (and internal `ILoggerFactory`) is disposed. 

You can avoid having to build the logger instance in each unit test method by deriving the test class from either `LoggingTestsBase` or `LoggingTestsBase<T>`. These classes provide the implementation to build the logger and dispose it. They also provide access to the `ITestOutputHelper` instance for writing directly to the test output.

```csharp
public class MyClassTests : LoggingTestsBase<MyClass>
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

## Output Formatting

The default formatting to the xUnit test results may not be what you want. You can define your own `ILogFormatter` class to control how the output looks. There is a configurable formatter for standard messages and another configurable formatter for scope start and end messages.

```csharp
public class MyFormatter : ILogFormatter
{
    public string Format(
        int scopeLevel,
        string categoryName,
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

        if (!string.IsNullOrEmpty(categoryName))
        {
            builder.Append($"{categoryName} ");
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

The custom `ILogFormatter` is defined on a `LoggingConfig` class that can be provided when creating a logger. The `MyConfig.Current` property above is there provide a clean way to share the config across test classes.

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

In the same way the format of start and end scope messages can be formatted by providing a custom formatter on `LoggingConfig.ScopeFormatter`.

[Back to top][0]

## Inspection

Using this library makes it really easy to output log messages from your code as part of the test results. You may want to also inspect the log messages written as part of the test assertions as well. 

The `BuildLogger` and `BuildLoggerFor<T>` extension methods support this by returning a `ICacheLogger` or `ICacheLogger<T>` respectively. The cache logger is a wrapper around the created logger and exposes all the log entries written by the test.

```csharp
using System;
using Neovolve.Logging.Xunit;
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

Perhaps you don't want to use the xUnit `ITestOutputHelper` but still want to use the `ICacheLogger` to run assertions over log messages written by the class under test. You can do this by creating a `CacheLogger` or `CacheLogger<T>` directly.

```csharp
using System;
using Neovolve.Logging.Xunit;
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

The `CacheLogger` class also supports a `LogWritten` event where `LogEntry` is provided in the event arguments.

[Back to top][0]

## Configured LoggerFactory

You may have an integration or acceptance test that requires additional configuration to the log providers on `ILoggerFactory` while also supporting the logging out to xUnit test results. You can do this by create a factory that is already configured with xUnit support.

You can get an xUnit configured `ILoggerFactory` by calling `output.BuildLoggerFactory()`.

```csharp
using System;
using Neovolve.Logging.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

public class MyClassTests
{
    private readonly ILogger _logger;

    public MyClassTests(ITestOutputHelper output)
    {
        var factory = output.BuildLoggerFactory();

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

The `BuildLoggerFactory` extension methods provide overloads to set the logging level or define [logging configuration][7].

[Back to top][0]

## Existing Loggers

Already have an existing logger and want the above cache support? Got you covered there too using the `WithCache()` method.

```csharp
using System;
using Neovolve.Logging.Xunit;
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

The `WithCache()` also supports `ILogger<T>`.

```csharp
using System;
using Neovolve.Logging.Xunit;
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

## Sensitive Values
The `LoggingConfig` class exposes a `SensitiveValues` property that holds a collection of strings. All sensitive values found in a log message or a start/end scope message will be masked out.

```csharp
public class ScopeScenarioTests : LoggingTestsBase<ScopeScenarioTests>
{
    private static readonly LoggingConfig _config = new LoggingConfig().Set(x => x.SensitiveValues.Add("secret"));

    public ScopeScenarioTests(ITestOutputHelper output) : base(output, _config)
    {
    }

    [Fact]
    public void TestOutputWritesScopeBoundariesUsingObjectsWithSecret()
    {
        Logger.LogCritical("Writing critical message with secret");
        Logger.LogDebug("Writing debug message with secret");
        Logger.LogError("Writing error message with secret");
        Logger.LogInformation("Writing information message with secret");
        Logger.LogTrace("Writing trace message with secret");
        Logger.LogWarning("Writing warning message with secret");

        var firstPerson = Model.Create<StructuredData>().Set(x => x.Email = "secret");

        using (Logger.BeginScope(firstPerson))
        {
            Logger.LogInformation("Inside first scope with secret");

            var secondPerson = Model.Create<StructuredData>().Set(x => x.FirstName = "secret");

            using (Logger.BeginScope(secondPerson))
            {
                Logger.LogInformation("Inside second scope with secret");
            }

            Logger.LogInformation("After second scope with secret");
        }

        Logger.LogInformation("After first scope with secret");
    }
```

The above test will render the following to the test output.

```
Critical [0]: Writing critical message with ****
Debug [0]: Writing debug message with ****
Error [0]: Writing error message with ****
Information [0]: Writing information message with ****
Trace [0]: Writing trace message with ****
Warning [0]: Writing warning message with ****
<Scope 1>
   Scope data: 
   {
     "DateOfBirth": "1972-10-07T16:35:31.2039449Z",
     "Email": "****",
     "FirstName": "Amos",
     "LastName": "Burton"
   }
   Information [0]: Inside first scope with ****
      <Scope 2>
      Scope data: 
      {
        "DateOfBirth": "1953-07-04T06:55:31.2333376Z",
        "Email": "james.holden@rocinante.space",
        "FirstName": "****",
        "LastName": "Holden"
      }
      Information [0]: Inside second scope with ****
   </Scope 2>
   Information [0]: After second scope with ****
</Scope 1>
Information [0]: After first scope with ****
```


[Back to top][0]

## Configuration

Logging configuration can be controled by using a `LoggingConfig` class as indicated in the [Output Formatting][3] section above. The following are the configuration options that can be set.

**Formatter**: Defines a custom formatter for rendering log messages to xUnit test output.

**ScopeFormatter**: Defines a custom formatter for rendering start and end scope messages to xUnit test output.

**IgnoreTestBoundaryException**: Defines whether exceptions thrown while logging outside of the test execution will be ignored.

**LogLevel**: Defines the minimum log level that will be written to the test output. This helps to limit the noise in test output when set to higher levels. Defaults to `LogLevel.Trace`.

**ScopePaddingSpaces**: Defines the number of spaces to use for indenting scopes.

**SensitiveValues**: Defines a collection of sensitive values that will be masked in the test output logging.

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
