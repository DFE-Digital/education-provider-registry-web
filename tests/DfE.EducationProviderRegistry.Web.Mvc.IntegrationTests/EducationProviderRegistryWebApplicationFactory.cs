using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests;

public sealed class EducationProviderRegistryWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly Action<IServiceCollection> _configureHostServices;

    public EducationProviderRegistryWebApplicationFactory(string connectionString, Action<IServiceCollection> configureHostServices)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;

        ArgumentNullException.ThrowIfNull(configureHostServices);
        _configureHostServices = configureHostServices;

        // default WebApplicationFactoryOptions to not auto-handle redirects
        ClientOptions.AllowAutoRedirect = false;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((services) =>
        {
            // TODO configure host
        });

        builder.ConfigureTestServices(_configureHostServices);
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
}