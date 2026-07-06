using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class EducationProviderRegistryDbContextFactory
{
    public static EducationProviderRegistryDbContext CreateDbContext()
    {
        DbContextOptions<EducationProviderRegistryDbContext> options =
            new DbContextOptionsBuilder<EducationProviderRegistryDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;

        return new EducationProviderRegistryDbContext(options);
    }
}
