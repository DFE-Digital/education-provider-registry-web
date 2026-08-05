using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions.Handlers;
using DfE.WebDriver;
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
                .UseDefaultServiceProvider((options) =>
                {
                    options.ValidateScopes = true;
                    options.ValidateOnBuild = true;
                })
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
        services.AddSingleton(t => t.GetRequiredService<IOptions<AccessibilityTestOptions>>().Value);

        services.AddOptions<ApplicationHostOptions>()
            .Bind(context.Configuration.GetSection(nameof(ApplicationHostOptions)))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton(t => t.GetRequiredService<IOptions<ApplicationHostOptions>>().Value);

        services.AddPostgresDatabase(context.Configuration.GetSection("DatabaseContainerOptions"));

        services.AddLogging((loggingBuilder) =>
            loggingBuilder.AddXunitOutput((optionsConfigure) =>
            {
                // TODO filter logging
            }));

        services.AddScoped<ApplicationHostedEnvironment>();

        services.AddScoped<Dictionary<string, Func<IAccessibilityScanActionHandler>>>((sp) =>
        {
            return new()
            {
                { "click", () => new ClickActionHandler() },
                { "enter", () => new SendKeysActionHandler() },
                { "navigate", () => new NavigateActionHandler() }
            };
        });

        services.AddWebDriver();
    }
}
