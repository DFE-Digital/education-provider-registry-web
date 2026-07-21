using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Options;
using MartinCostello.Logging.XUnit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit.DependencyInjection.Logging;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests;

// Mark members as static - Startup is instantiated by XUnit.DependencyInjection and instance is expected

public sealed class Startup
{
    public void ConfigureHost(IHostBuilder hostBuilder) =>
            hostBuilder
                .ConfigureHostConfiguration(builder => { })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    builder.AddJsonFile($"appsettings.{context.HostingEnvironment}.json", optional: true, reloadOnChange: true);
                    builder.AddEnvironmentVariables();
                });

    public void ConfigureServices(IServiceCollection services, HostBuilderContext context)
    {
        services.AddOptions<XUnitLoggerOptions>();

        services
            .AddOptions<AccessibilityTestOptions>()
            .Bind(context.Configuration.GetSection(nameof(AccessibilityTestOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<AccessibilityTestOptions>(t => t.GetRequiredService<IOptions<AccessibilityTestOptions>>().Value);

        services.AddOptions<ApplicationHostOptions>()
            .Bind(context.Configuration.GetSection(nameof(ApplicationHostOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ApplicationHostOptions>(t => t.GetRequiredService<IOptions<ApplicationHostOptions>>().Value);

        services.AddPostgresDatabase(context.Configuration.GetSection("DatabaseContainerOptions"));

        services.AddLogging((loggingBuilder) =>
            loggingBuilder.AddXunitOutput((optionsConfigure) =>
            {
                // TODO filter logging
            }));

        services.AddSingleton<ApplicationHostedEnvironment>();
    }
}
