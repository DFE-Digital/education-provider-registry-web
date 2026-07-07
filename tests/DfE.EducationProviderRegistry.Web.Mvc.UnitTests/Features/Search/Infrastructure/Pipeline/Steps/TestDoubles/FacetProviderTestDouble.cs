using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers;
using Moq;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.Pipeline.Steps.TestDoubles;

[ExcludeFromCodeCoverage]
internal class FacetProviderTestDouble
{
    public static Mock<IFacetProvider> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<IFacetProvider>
        MockFor(IReadOnlyList<FacetResult> facetResults)
    {
        Mock<IFacetProvider> facetProviderMock = Mock();

        facetProviderMock
            .Setup(facetProvider => facetProvider.GetFacetsAsync(
                It.IsAny<ReadOnlyCollection<string>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(facetResults);

        return facetProviderMock;
    }
}
