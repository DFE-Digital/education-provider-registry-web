using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry.BuilderHandler;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Extensions;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Actions.Handlers;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Container;
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

        IConfiguration applicationHostOptions = context.Configuration.GetSection(nameof(ApplicationHostOptions));

        services.AddOptions<ApplicationHostOptions>()
            .Bind(applicationHostOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton(t => t.GetRequiredService<IOptions<ApplicationHostOptions>>().Value);

        services.AddTransient<DatabaseConnectionStringBuilderHandler>();
        services.AddTransient<HttpWaitStrategyBuilderHandler>();

        services.AddSingleton<Dictionary<string, IReadOnlyCollection<Func<IConfigureContainerBuilderHandler<ContainerBuilder>>>>>(sp =>
        {
            return new()
            {
                { "epr-web",
                    [
                        () => sp.GetRequiredService<DatabaseConnectionStringBuilderHandler>(),
                        () => sp.GetRequiredService<HttpWaitStrategyBuilderHandler>()
                    ]
                }
            };
        });

        services.AddContainer(
            key: "epr-web",
            configuration: applicationHostOptions);

        services.AddPostgres(context.Configuration);

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
