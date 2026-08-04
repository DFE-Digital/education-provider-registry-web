using DfE.Core.Libraries.Testing;
using DfE.Core.Libraries.Testing.Services;
using DfE.EducationProviderRegistry.Web.Mvc.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Extensions;

public sealed class DatabaseServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPostgresDatabase_ThrowsInvalidOperationException_WhenConnectionStringIsMissing()
    {
        // arrange
        IConfiguration configuration = ConfigurationDefault.Create();

        IServiceCollection services = ServiceCollectionDefaults.Create();

        // act/assert
        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => services.AddPostgresDatabase(configuration));

        Assert.Equal(
            "Configuration value 'eprweb_eprdat_dotnet_db_connection' is missing.",
            exception.Message);
    }

    [Fact]
    public void AddPostgresDatabase_ThrowsArgumentException_WhenConnectionStringIsInvalid()
    {
        // arrange
        Dictionary<string, string?> configurationValues =
            new()
            {
                ["eprweb_eprdat_dotnet_db_connection"] =
                    "Host=localhost;Port=invalid"
            };

        IConfiguration configuration =
            ConfigurationDefault.CreateBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();

        IServiceCollection services = ServiceCollectionDefaults.Create();

        // act/assert
        Assert.Throws<ArgumentException>(
            () => services.AddPostgresDatabase(configuration));
    }

    [Fact]
    public void AddPostgresDatabase_ReturnsServiceCollection_WhenConnectionStringIsValid()
    {
        // arrange
        Dictionary<string, string?> configurationValues =
            new()
            {
                ["eprweb_eprdat_dotnet_db_connection"] =
                    "Host=localhost;Database=test;Username=user;Password=password"
            };

        IConfiguration configuration =
            ConfigurationDefault.CreateBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();

        IServiceCollection services = ServiceCollectionDefaults.Create();

        // act
        IServiceCollection result =
            services.AddPostgresDatabase(configuration);

        // assert
        Assert.Same(
            services,
            result);
    }
}
