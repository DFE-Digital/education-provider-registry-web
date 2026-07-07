using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace DfE.EducationProviderRegistry.Web.Mvc.UnitTests.Features.Search.Infrastructure.TestDoubles;

[ExcludeFromCodeCoverage]
internal static class SearchResultsMapperTestDouble
{
    public static Mock<IMapper<
        SearchPipelineContext,
        SearchResults<EstablishmentSearchResults, SearchFacets>>> Mock() => new(MockBehavior.Strict);

    public static Mock<IMapper<
        SearchPipelineContext,
        SearchResults<EstablishmentSearchResults, SearchFacets>>> MockFor(
        SearchResults<EstablishmentSearchResults, SearchFacets> searchResults)
    {
        var searchResultsMapper = Mock();

        searchResultsMapper
            .Setup(mapper =>
                    mapper.Map(It.IsAny<SearchPipelineContext>()))
                .Returns(searchResults);

        return searchResultsMapper;
    }
}
