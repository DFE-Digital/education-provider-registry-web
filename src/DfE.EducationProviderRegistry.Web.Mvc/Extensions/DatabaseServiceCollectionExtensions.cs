using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DfE.EducationProviderRegistry.Web.Mvc.Extensions;

internal static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "eprweb_eprdat_dotnet_db_connection";

        string connectionString =
            configuration[ConnectionStringKey] ??
                throw new InvalidOperationException($"Configuration value '{ConnectionStringKey}' is missing.");

        // Validate connectionString
        _ = new NpgsqlConnectionStringBuilder(connectionString);

        services.AddDbContextFactory<EducationProviderRegistryDbContext>(options =>
        {
            options.UseNpgsql(connectionString)
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors()
                   .LogTo(Console.WriteLine, LogLevel.Information);
        });

        return services;
    }
}
