using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Abstractions;
using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests;

public abstract class WebApplicationFactoryBaseIntegrationTest : IntegrationTestsBase, IAsyncLifetime
{
    private IDatabase? _db;
    private readonly IPostgresDatabaseProvider _dbProvider;
    private string? _postgresConnectionString;

    public WebApplicationFactoryBaseIntegrationTest(IServiceProvider provider)
    {
        _dbProvider = provider.GetRequiredService<IPostgresDatabaseProvider>();

    }

#nullable disable
    protected EducationProviderRegistryWebApplicationFactory Factory { get; private set; }
#nullable enable

    public async ValueTask InitializeAsync()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;

        const string dbKey = "postgres";

        _db = await _dbProvider.GetDatabaseAsync(dbKey, ct);
        await _db.StartAsync(ct);

        _postgresConnectionString = await _dbProvider.GetConnectionStringAsync(dbKey, cancellationToken: ct);

        Factory = new(_postgresConnectionString);
    }

    protected override async Task BeforeDisposeAsync()
    {
        if (_db != null)
        {
            await _db.DisposeAsync();
        }
    }
}
