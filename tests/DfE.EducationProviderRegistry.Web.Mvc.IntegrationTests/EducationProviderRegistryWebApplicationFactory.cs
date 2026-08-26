using DfE.Core.Libraries.IntegrationTests.Database.Postgres.Container.Provider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests;

internal class EducationProviderRegistryWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly IPostgresContainerConnectionStringProvider _provider;

    public EducationProviderRegistryWebApplicationFactory(IPostgresContainerConnectionStringProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        _provider = provider;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(
                [
                    new("eprweb_eprdat_dotnet_db_connection", _provider.GetConnectionString())
                ]);
        });

        builder.ConfigureServices((services) =>
        {

        });
    }

    public HttpClient CreateDefaultedHttpClient(Action<WebApplicationFactoryClientOptions>? configure = null)
    {
        WebApplicationFactoryClientOptions options = new()
        {
            AllowAutoRedirect = false,
        };

        configure?.Invoke(options);

        return base.CreateClient(options);
    }

    protected override void ConfigureClient(HttpClient client)
    {
        client.BaseAddress = new Uri("https://localhost");
    }
}