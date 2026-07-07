using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchProviderTestDouble
{
    public static Mock<ISearchProvider<Establishment>> Mock() =>
        new(MockBehavior.Strict);

    public static Mock<ISearchProvider<Establishment>>
        MockFor(List<Establishment> establishments)
    {
        Mock<ISearchProvider<Establishment>> searchProviderMock = Mock();

        searchProviderMock
            .Setup(searchProvider =>
                searchProvider.GetMatchingIdsAsync(
                It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<IReadOnlyList<SearchFilterRequest>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(establishments);

        return searchProviderMock;
    }
}