namespace Neovolve.Logging.Xunit.UnitTests
{
    using System;
    using FluentAssertions;
    using global::Xunit;
    using global::Xunit.Abstractions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NSubstitute;

    public class LoggingBuilderExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public LoggingBuilderExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void AddXunitAddsProviderToBuilder()
        {
            var services = Substitute.For<IServiceCollection>();
            var builder = Substitute.For<ILoggingBuilder>();

            builder.Services.Returns(services);

            builder.AddXunit(_output);

            services.Received().Add(Arg.Is<ServiceDescriptor>(x => x.ServiceType == typeof(ILoggerProvider)));
            services.Received()
                .Add(Arg.Is<ServiceDescriptor>(x => x.ImplementationInstance is TestOutputLoggerProvider));
        }

        [Fact]
        public void AddXunitAddsProviderWithLoggingConfigToBuilder()
        {
            var config = new LoggingConfig();

            var services = Substitute.For<IServiceCollection>();
            var builder = Substitute.For<ILoggingBuilder>();

            builder.Services.Returns(services);

            builder.AddXunit(_output, config);

            services.Received().Add(Arg.Is<ServiceDescriptor>(x => x.ServiceType == typeof(ILoggerProvider)));
            services.Received()
                .Add(Arg.Is<ServiceDescriptor>(x => x.ImplementationInstance is TestOutputLoggerProvider));
        }

        [Fact]
        public void AddXunitThrowsExceptionWithNullBuilder()
        {
            ILoggingBuilder builder = null!;

            var action = () => builder.AddXunit(_output);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddXunitThrowsExceptionWithNullOutput()
        {
            var builder = Substitute.For<ILoggingBuilder>();

            var action = () => builder.AddXunit(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}