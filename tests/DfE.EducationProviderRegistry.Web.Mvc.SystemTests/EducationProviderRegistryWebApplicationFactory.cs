using DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DfE.EducationProviderRegistry.Web.Mvc.SystemTests;

public sealed class EducationProviderRegistryWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly Action<IConfigurationBuilder>? _configureConfiguration;
    private readonly Action<IServiceCollection> _configureHostServices;

    public EducationProviderRegistryWebApplicationFactory(
        string connectionString,
        Action<IServiceCollection>? configureHostServices = null,
        Action<IConfigurationBuilder>? configureConfiguration = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
        _configureConfiguration = configureConfiguration;
        _configureHostServices = configureHostServices ??= services => { };



        // Removes https redirect warnings 
        ClientOptions.BaseAddress = new("https://localhost");
        // default WebApplicationFactoryOptions to not auto-handle redirects
        ClientOptions.AllowAutoRedirect = false;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((services) =>
        {
            services.AddTestAntiForgeryTokenServices();
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

            _configureConfiguration?.Invoke(config);
        });

        return base.CreateHost(builder);
    }
}
