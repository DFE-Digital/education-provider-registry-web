using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Provider;
using DotNet.Testcontainers.Containers;

namespace DfE.EducationProviderRegistry.Web.SharedTests.ApplicationContainer;

public sealed class ApplicationHostedEnvironment
{
    private IDatabase? _database;
    private IContainer? _applicationContainer;
    private readonly IContainerRegistry _containerRegistry;
    private readonly IPostgresDatabaseProvider _dbProvider;

    public ApplicationHostedEnvironment(
        IContainerRegistry containerRegistry,
        IPostgresDatabaseProvider dbProvider)
    {
        _containerRegistry = containerRegistry;
        _dbProvider = dbProvider;
    }

    public async Task InitialiseAsync(
        CancellationToken ct = default)
    {
        _database = await _dbProvider.GetDatabaseAsync("postgres", ct);
        await _database.StartAsync(ct);

        _applicationContainer = await _containerRegistry.GetOrCreateContainerAsync("epr-web", ct);
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