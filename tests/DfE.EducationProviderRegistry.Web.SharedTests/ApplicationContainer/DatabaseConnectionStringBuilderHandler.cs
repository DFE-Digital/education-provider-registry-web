using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry.BuilderHandler;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;
using DotNet.Testcontainers.Builders;

namespace DfE.EducationProviderRegistry.Web.SharedTests.ApplicationContainer;

internal sealed class DatabaseConnectionStringBuilderHandler
    : IConfigureContainerBuilderHandler<ContainerBuilder>
{
    private readonly IPostgresDatabaseProvider _provider;

    public DatabaseConnectionStringBuilderHandler(IPostgresDatabaseProvider provider)
    {
        _provider = provider;
    }

    public async ValueTask<ContainerBuilder> HandleAsync(
        ContainerBuilder builder,
        CancellationToken cancellationToken)
    {
        string connectionString = await _provider.GetConnectionStringAsync("postgres", cancellationToken: cancellationToken);
        return builder.WithEnvironment(name: "eprweb_eprdat_dotnet_db_connection", value: connectionString);
    }
}