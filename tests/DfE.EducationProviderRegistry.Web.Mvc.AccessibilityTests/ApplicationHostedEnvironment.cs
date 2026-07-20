using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Options;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests;

public sealed class ApplicationHostedEnvironment
{
    private readonly IDatabaseFactory _databaseFactory;
    private readonly ApplicationHostOptions _options;
    
    public ApplicationHostedEnvironment(
        IDatabaseFactory databaseFactory,
        ApplicationHostOptions options)
    {
        _databaseFactory = databaseFactory;
        _options = options;
    }

    public IDatabase Database { get; private set; } = null!;
    public IContainer Application { get; private set; } = null!;

    public async Task InitialiseAsync(
        CancellationToken ct = default)
    {
        Database = await _databaseFactory.CreateAsync(ct);

        // TODO tests - share same Web container - Startup instead of ICollectionFixture to run in parallel?
        Application = new ContainerBuilder(_options.Container.Image)
          .WithPortBinding(8080, true)
          // Wait until the HTTP endpoint of the container is available. // TODO check Health
          .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(8080)))
          // Build the container configuration.
          .Build();

        // Start the container.
        await Application.StartAsync(ct);
    }

    public Uri GetApplicationUrl() => new($"http://localhost:{Application.GetMappedPublicPort(8080)}");
}