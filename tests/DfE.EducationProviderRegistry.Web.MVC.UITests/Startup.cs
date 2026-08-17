using MartinCostello.Logging.XUnit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests;

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

        IConfiguration applicationHostOptions = context.Configuration.GetSection(nameof(ApplicationHostOptions));

        services.AddOptions<ApplicationHostOptions>()
            .Bind(applicationHostOptions)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton(t => t.GetRequiredService<IOptions<ApplicationHostOptions>>().Value);

        services.AddScoped<DatabaseConnectionStringBuilderHandler>();
        services.AddScoped<HttpWaitStrategyBuilderHandler>();

        services.AddScoped<Dictionary<string, IReadOnlyCollection<Func<IConfigureContainerBuilderHandler<ContainerBuilder>>>>>(sp =>
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
