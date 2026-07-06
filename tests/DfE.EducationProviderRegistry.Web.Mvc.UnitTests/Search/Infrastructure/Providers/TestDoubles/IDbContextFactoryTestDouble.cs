using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Search.Infrastructure.Providers.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class IDbContextFactoryTestDouble
{
    public static Mock<IDbContextFactory<EducationProviderRegistryDbContext>> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<IDbContextFactory<EducationProviderRegistryDbContext>>
        MockFor(EducationProviderRegistryDbContext dbContext)
    {
        Mock<IDbContextFactory<EducationProviderRegistryDbContext>> contextFactoryMock = Mock();

        contextFactoryMock.
            Setup(factory =>
                factory.CreateDbContextAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(dbContext)
                    .Verifiable();

        return contextFactoryMock;
    }
}
