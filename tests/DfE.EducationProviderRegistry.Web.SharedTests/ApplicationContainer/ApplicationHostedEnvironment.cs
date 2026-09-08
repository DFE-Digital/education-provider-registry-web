using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;
using DotNet.Testcontainers.Containers;
using System.Reflection.Metadata.Ecma335;

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

        UriBuilder builder = new()
        {
            Scheme = "http",
            Host = "localhost",
            Port = _applicationContainer.GetMappedPublicPort(8080)
        };

        return builder.Uri;
    }

    public async Task<string> GetApplicationLogsAsync()
    {
        (string stdout, string stderr) = await _applicationContainer!.GetLogsAsync();

        return $"""
        === STDOUT ===
        {stdout}

        === STDERR ===
        {stderr}
        """;
    }
}