using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests;

public sealed class ApplicationHostedEnvironment
{
    private IDatabase? _database;
    private IContainer? _applicationContainer;
    private readonly IDatabaseFactory _databaseFactory;
    private readonly IContainerRegistry _containerRegistry;
    private readonly ApplicationHostOptions _options;
    private readonly ContainerOptions _dbContainerOptions;
    private readonly PostgresDatabaseOptions _dbOptions;

    public ApplicationHostedEnvironment(
        ApplicationHostOptions applicationOptions,
        ContainerOptions dbContainerOptions,
        PostgresDatabaseOptions dbOptions,
        IDatabaseFactory databaseFactory,
        IContainerRegistry containerRegistry)
    {
        _options = applicationOptions;
        _dbContainerOptions = dbContainerOptions;
        _dbOptions = dbOptions;

        _databaseFactory = databaseFactory;
        _containerRegistry = containerRegistry;
    }

    public async Task InitialiseAsync(
        CancellationToken ct = default)
    {
        _database = await _databaseFactory.CreateAsync(ct);
        await _database.StartAsync(ct);

        const int postgresContainerPort = 5432;

        NpgsqlConnectionStringBuilder containerNetworkConnectionStringBuilder = new()
        {
            Host = _dbContainerOptions.Networks.First().Aliases.First(), // resolve alias to enable connection
            Port = postgresContainerPort, // Internal postgres port as container-container
            Database = _dbOptions.Database,
            Username = _dbOptions.Username,
            Password = _dbOptions.Password
        };

        ContainerBuilder builder =
            new ContainerBuilder(_options.Container.Image)
                .WithExposedPorts<ContainerBuilder, IContainer, IContainerConfiguration>(_options.Container.PortMappings ?? [])
                .WithEnvironment("eprweb_eprdat_dotnet_db_connection", containerNetworkConnectionStringBuilder.ConnectionString)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort((ushort)_options.Container.PortMappings!.First().ContainerPort)));

        builder =
            await builder
                .WithNetworksAsync<ContainerBuilder, IContainer, IContainerConfiguration>(
                    _options.Container?.Networks, _containerRegistry);

        _applicationContainer = builder.Build();

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