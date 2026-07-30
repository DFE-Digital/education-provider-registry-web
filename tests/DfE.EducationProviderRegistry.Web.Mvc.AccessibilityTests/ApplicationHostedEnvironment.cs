using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Options;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Npgsql;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests;

public sealed class ApplicationHostedEnvironment
{
    private IDatabase? _database;
    private IContainer? _applicationContainer;
    private readonly IDatabaseFactory _databaseFactory;
    private readonly ApplicationHostOptions _options;
    private readonly ContainerOptions _dbContainerOptions;
    private readonly PostgresDatabaseOptions _dbOptions;
    private readonly INetwork _containerNetwork;

    public ApplicationHostedEnvironment(
        IDatabaseFactory databaseFactory,
        ApplicationHostOptions options,
        INetwork containerNetwork,
        ContainerOptions dbContainerOptions,
        PostgresDatabaseOptions dbOptions)
    {
        _databaseFactory = databaseFactory;
        _options = options;
        _containerNetwork = containerNetwork;
        _dbContainerOptions = dbContainerOptions;
        _dbOptions = dbOptions;
    }

    public async Task InitialiseAsync(
        CancellationToken ct = default)
    {
        _database = await _databaseFactory.CreateAsync(ct);

        const int postgresContainerPort = 5432;

        NpgsqlConnectionStringBuilder containerNetworkConnectionStringBuilder = new()
        {
            Host = _dbContainerOptions.HostName,
            Port = postgresContainerPort, // Internal postgres port as container-container
            Database = _dbOptions.Database,
            Username = _dbOptions.Username,
            Password = _dbOptions.Password
        };

        _applicationContainer =
            new ContainerBuilder(_options.Container.Image)
                .WithExposedPorts<ContainerBuilder, IContainer, IContainerConfiguration>(_options.Container.PortMappings ?? [])
                .WithEnvironment("eprweb_eprdat_dotnet_db_connection", containerNetworkConnectionStringBuilder.ConnectionString)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort((ushort)_options.Container.PortMappings!.First().ContainerPort)))
                .WithNetwork(_containerNetwork)
                .Build();

        // Start the container.
        await _applicationContainer.StartAsync(ct);
    }

    public Uri GetApplicationUrl()
    {
        if (_applicationContainer == null)
        {
            throw new ArgumentException($"Host environment has not been started with {nameof(InitialiseAsync)}");
        }

        return new($"http://localhost:{_applicationContainer.GetMappedPublicPort(8080)}");
    }

    public async Task<string> GetLogsAsync()
    {
        (string stdout, string stderr) logs =
            await _applicationContainer!.GetLogsAsync();

        return $"""
        === STDOUT ===
        {logs.stdout}

        === STDERR ===
        {logs.stderr}
        """;
    }
}