using DfE.Core.Libraries.IntegrationTests.Abstractions.Containers.Registry;
using DfE.EducationProviderRegistry.Core.Query.Test.Database;
using DotNet.Testcontainers.Containers;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.ApplicationContainer;

public sealed class ApplicationHostedEnvironment : IAsyncDisposable
{
    private readonly IContainerRegistry _containerRegistry;
    private IContainer? _applicationContainer;


    public ApplicationHostedEnvironment(
        IContainerRegistry containerRegistry,
        EducationProviderRegistryDatabaseFixture dbFixture)
    {
        ArgumentNullException.ThrowIfNull(containerRegistry);
        ArgumentNullException.ThrowIfNull(dbFixture);

        _containerRegistry = containerRegistry;
        DatabaseFixture = dbFixture;
    }

    public EducationProviderRegistryDatabaseFixture DatabaseFixture { get; }

    public async Task InitialiseAsync(CancellationToken ct = default)
    {
        await DatabaseFixture.StartAsync(ct: ct);

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

    public async ValueTask DisposeAsync()
    {
        await DatabaseFixture.DisposeAsync();
    }
}
