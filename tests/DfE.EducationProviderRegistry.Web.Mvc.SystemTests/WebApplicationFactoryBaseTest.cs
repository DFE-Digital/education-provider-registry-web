using DfE.Core.Libraries.IntegrationTests.Abstractions;
using DfE.EducationProviderRegistry.Core.Query.Test.Database;
using DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.WebApplicationFactory.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.Mvc.SystemTests;

public abstract class WebApplicationFactoryBaseTest : IntegrationTestsBase, IAsyncLifetime
{
    protected WebApplicationFactoryBaseTest(IServiceProvider provider)
    {
        DatabaseFixture = provider.GetRequiredService<EducationProviderRegistryDatabaseFixture>();
    }

    protected EducationProviderRegistryDatabaseFixture DatabaseFixture { get; }
#nullable disable
    protected EducationProviderRegistryWebApplicationFactory Factory { get; private set; }
#nullable enable

    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services
            .WithClarity()
            .WithGoogleTagManager();
    }

    public async ValueTask InitializeAsync()
    {
        CancellationToken ct = TestContext.Current.CancellationToken;

        await DatabaseFixture.StartAsync(ct: ct);
        Factory = new(DatabaseFixture.ConnectionString, ConfigureServices);
    }


    protected async override Task DisposeApplicationAsync()
    {
        await DatabaseFixture.DisposeAsync();
        await Factory.DisposeAsync();
    }
}