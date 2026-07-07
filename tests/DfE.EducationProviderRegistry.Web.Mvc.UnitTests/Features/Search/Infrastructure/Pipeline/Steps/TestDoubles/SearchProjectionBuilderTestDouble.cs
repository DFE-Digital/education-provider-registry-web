using DfE.EducationProviderRegistry.Data.DatabaseModels.Context;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Providers.Projections;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Pipeline.Steps.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchProjectionBuilderTestDouble
{
    public static Mock<ISearchProjectionBuilder<Establishment>> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<ISearchProjectionBuilder<Establishment>>
        MockFor(
            EducationProviderRegistryDbContext dbContext,
            IQueryable<Establishment> baseQuery
        )
    {
        Mock<ISearchProjectionBuilder<Establishment>> projectionBuilderMock = Mock();

        projectionBuilderMock
            .Setup(projectionBuilder =>
                projectionBuilder.Build(dbContext))
            .Returns(baseQuery);

        return projectionBuilderMock;
    }
}
