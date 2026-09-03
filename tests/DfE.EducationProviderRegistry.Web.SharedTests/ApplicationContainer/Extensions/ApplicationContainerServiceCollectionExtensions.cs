using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Extensions;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry.BuilderHandler;
using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DfE.EducationProviderRegistry.Web.SharedTests.ApplicationContainer.Extensions;

public static class ApplicationContainerServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationContainer(this IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<DatabaseConnectionStringBuilderHandler>();
        services.TryAddScoped<HttpWaitStrategyBuilderHandler>();

        services.AddScoped<Dictionary<string, Func<IReadOnlyCollection<IConfigureContainerBuilderHandler<ContainerBuilder>>>>>(sp =>
        {
            return new()
            {
                { "epr-web",
                     () => [
                        sp.GetRequiredService<DatabaseConnectionStringBuilderHandler>(),
                        sp.GetRequiredService<HttpWaitStrategyBuilderHandler>()
                    ]
                }
            };
        });

        services.AddContainer(
            key: "epr-web",
            configuration: configuration);

        services.AddScoped<ApplicationHostedEnvironment>();

        return services;
    }
}
