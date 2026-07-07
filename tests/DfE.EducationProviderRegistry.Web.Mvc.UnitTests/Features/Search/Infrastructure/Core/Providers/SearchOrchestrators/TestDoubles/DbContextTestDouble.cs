using Microsoft.EntityFrameworkCore;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Core.Providers.SearchOrchestrators.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class DbContextTestDouble
{
    public static DbContext BuildFakeDbContext() =>
        new Mock<DbContext>(
            new DbContextOptions<DbContext>()).Object;
}
