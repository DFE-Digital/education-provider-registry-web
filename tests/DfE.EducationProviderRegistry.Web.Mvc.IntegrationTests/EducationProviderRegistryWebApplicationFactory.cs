using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests;

public sealed class EducationProviderRegistryWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public EducationProviderRegistryWebApplicationFactory(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((services) =>
        {
            // TODO configure host
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Application binds config directly limitation requires host configuration https://github.com/dotnet/aspnetcore/issues/37680
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection([
                new("eprweb_eprdat_dotnet_db_connection", _connectionString)
            ]);
        });

        return base.CreateHost(builder);

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
}