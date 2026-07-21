using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests.Options;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace DfE.EducationProviderRegistry.Web.Mvc.AccessibilityTests;

public sealed class ApplicationHostedEnvironment
{
    private IDatabase? _database;
    private IContainer? _applicationContainer;
    private readonly IDatabaseFactory _databaseFactory;
    private readonly ApplicationHostOptions _options;

    public ApplicationHostedEnvironment(
        IDatabaseFactory databaseFactory,
        ApplicationHostOptions options)
    {
        _databaseFactory = databaseFactory;
        _options = options;
    }

    public async Task InitialiseAsync(
        CancellationToken ct = default)
    {
        _database = await _databaseFactory.CreateAsync(ct);

        _applicationContainer =
            new ContainerBuilder(_options.Container.Image)
                .WithExposedPorts<ContainerBuilder, IContainer, IContainerConfiguration>(_options.Container.PortMappings ?? [])
                .WithEnvironment("eprweb_eprdat_dotnet_db_connection", _database.ConnectionString)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort((ushort)_options.Container.PortMappings!.First().ContainerPort)))
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
}