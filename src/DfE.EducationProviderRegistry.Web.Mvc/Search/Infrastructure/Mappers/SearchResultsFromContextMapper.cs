using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Mappers;

public sealed class SearchResultsFromContextMapper
    : IMapper<SearchPipelineContext, SearchResults<EstablishmentSearchResults, SearchFacets>>
{
    public SearchResults<EstablishmentSearchResults, SearchFacets> Map(SearchPipelineContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        EstablishmentSearchResult[] mapped = context.Get<EstablishmentSearchResult[]>();
        List<SearchFacet> facets = context.Get<List<SearchFacet>>();

        ArgumentNullException.ThrowIfNull(mapped);
        ArgumentNullException.ThrowIfNull(facets);

        EstablishmentSearchResults wrappedResults = new(mapped);
        SearchFacets wrappedFacets = new(facets);

        return new SearchResults<EstablishmentSearchResults, SearchFacets>
        {
            Results = wrappedResults,
            FacetResults = wrappedFacets
        };
    }
}
