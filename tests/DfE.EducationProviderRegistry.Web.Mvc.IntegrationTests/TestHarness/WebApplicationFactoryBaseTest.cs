using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;
using DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness.Antiforgery;
using DfE.EducationProviderRegistry.Web.Mvc.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.TestHarness;

public abstract class WebApplicationFactoryBaseTest : IntegrationTestsBase, IAsyncLifetime
{
    private IDatabase? _db;
    private readonly IPostgresDatabaseProvider _dbProvider;
    private string? _postgresConnectionString;

    protected WebApplicationFactoryBaseTest(IServiceProvider provider)
    {
        _dbProvider = provider.GetRequiredService<IPostgresDatabaseProvider>();
    }

#nullable disable
    protected EducationProviderRegistryWebApplicationFactory Factory { get; private set; }
#nullable enable

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        // Valid clarity
        services.PostConfigure<ClaritySettings>((opts) =>
        {
            opts.Enabled = true;
            opts.ProjectId = "STUB-PROJECTID";
        });

        // Valid GTM
        services.PostConfigure<GoogleAnalyticsSettings>((opts) =>
        {
            opts.ContainerId = "STUB-GTM-CONTAINERID";
        });
    }


    public async ValueTask InitializeAsync()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;

        const string dbKey = "postgres";

        _db = await _dbProvider.GetDatabaseAsync(dbKey, ct);
        await _db.StartAsync(ct);

        _postgresConnectionString = await _dbProvider.GetConnectionStringAsync(dbKey, cancellationToken: ct);

        Factory = new(_postgresConnectionString, ConfigureServices);
    }

    protected override async Task BeforeDisposeAsync()
    {
        if (_db != null)
        {
            await _db.DisposeAsync();
        }

        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }
    }
}