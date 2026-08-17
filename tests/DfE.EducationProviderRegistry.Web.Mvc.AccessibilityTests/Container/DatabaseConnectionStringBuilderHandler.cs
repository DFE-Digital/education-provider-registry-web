using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry.BuilderHandler;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Options;
using Microsoft.Extensions.Options;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Container;

internal sealed class DatabaseConnectionStringBuilderHandler
    : IConfigureContainerBuilderHandler<ContainerBuilder>
{
    private readonly IOptionsMonitor<PostgresContainerOptions> _options;

    public DatabaseConnectionStringBuilderHandler(
        IOptionsMonitor<PostgresContainerOptions> options)
    {
        _options = options;
    }

    public ValueTask<ContainerBuilder> HandleAsync(
        ContainerBuilder builder,
        CancellationToken cancellationToken)
    {
        const string postgresContainerName = "postgres";

        PostgresContainerOptions dbOptions = _options.Get(postgresContainerName);

        NpgsqlConnectionStringBuilder connectionStringBuilder = new()
        {
            Host = dbOptions.Container!.Networks!.First().Aliases.First(),
            Port = 5432,
            Database = dbOptions.Database!.Name,
            Username = dbOptions.Database.Username,
            Password = dbOptions.Database.Password
        };

        return ValueTask.FromResult(
            builder.WithEnvironment(
                "eprweb_eprdat_dotnet_db_connection",
                connectionStringBuilder.ConnectionString));
    }
}