using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry.BuilderHandler;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Provider;
using DotNet.Testcontainers.Builders;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Container;

internal sealed class DatabaseConnectionStringBuilderHandler
    : IConfigureContainerBuilderHandler<ContainerBuilder>
{
    private readonly IPostgresContainerConnectionStringProvider _provider;

    public DatabaseConnectionStringBuilderHandler(IPostgresContainerConnectionStringProvider provider)
    {
        _provider = provider;
    }

    public ValueTask<ContainerBuilder> HandleAsync(
        ContainerBuilder builder,
        CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(
            builder.WithEnvironment(
                name: "eprweb_eprdat_dotnet_db_connection",
                value: _provider.GetConnectionString()));
    }
}