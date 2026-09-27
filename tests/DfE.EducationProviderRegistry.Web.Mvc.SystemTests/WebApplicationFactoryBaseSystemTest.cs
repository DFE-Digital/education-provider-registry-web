using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.EducationProviderRegistry.Core.Query.Test.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.Mvc.SystemTests;

public abstract class WebApplicationFactoryBaseSystemTest : IntegrationTestsBase, IAsyncLifetime
{
    protected WebApplicationFactoryBaseSystemTest(IServiceProvider provider)
    {
        DatabaseFixture = provider.GetRequiredService<EducationProviderRegistryDatabaseFixture>();
    }

    protected EducationProviderRegistryDatabaseFixture DatabaseFixture { get; }
#nullable disable
    protected EducationProviderRegistryWebApplicationFactory Factory { get; private set; }
#nullable enable

    protected virtual void ConfigureApplicationServices(IServiceCollection services) { }

    protected virtual void ConfigureApplicationConfiguration(IConfigurationBuilder configurationBuilder) { }

    public async ValueTask InitializeAsync()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;

        await DatabaseFixture.StartAsync(ct: ct);

        Factory = new(
            connectionString: DatabaseFixture.ConnectionString,
            configureHostServices: ConfigureApplicationServices,
            configureConfiguration: ConfigureApplicationConfiguration);
    }

    protected sealed async override Task DisposeApplicationAsync()
    {
        await DatabaseFixture.DisposeAsync();
        await Factory.DisposeAsync();
    }
}
