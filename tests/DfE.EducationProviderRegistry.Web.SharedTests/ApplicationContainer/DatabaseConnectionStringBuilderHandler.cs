using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Options.Container;
using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry.BuilderHandler;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;
using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Web.SharedTests.ApplicationContainer;

internal sealed class DatabaseConnectionStringBuilderHandler
    : IConfigureContainerBuilderHandler<ContainerBuilder>
{
    private readonly IOptionsMonitor<ContainerOptions> _options;
    private readonly IPostgresDatabaseProvider _provider;

    public DatabaseConnectionStringBuilderHandler(IPostgresDatabaseProvider provider, IOptionsMonitor<ContainerOptions> options)
    {
        _provider = provider;
        _options = options;
    }

    public async ValueTask<ContainerBuilder> HandleAsync(
        ContainerBuilder builder,
        CancellationToken cancellationToken)
    {
        // assumption single network for application-container
        string networkName = _options.Get("epr-web").Networks.First().Key;

        string connectionString =
            await _provider.GetConnectionStringAsync(
                key: "postgres",
                networkName: networkName,
                cancellationToken: cancellationToken);

        return builder.WithEnvironment(name: "eprweb_eprdat_dotnet_db_connection", value: connectionString);
    }
}