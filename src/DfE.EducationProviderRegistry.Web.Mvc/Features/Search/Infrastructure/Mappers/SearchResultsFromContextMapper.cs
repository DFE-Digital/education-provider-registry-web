using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Mappers;

/// <summary>
/// Maps a <see cref="SearchPipelineContext"/> into a strongly typed
/// <see cref="SearchResults{TResults, TFacets}"/> instance containing
/// establishment search results and facet results.
/// </summary>
/// <remarks>
/// This mapper performs strict null validation.  
/// The pipeline context must contain:
/// <list type="bullet">
/// <item><description><see cref="EstablishmentSearchResult"/>[]</description></item>
/// <item><description><see cref="List{SearchFacet}"/></description></item>
/// </list>
/// If either component is missing, an <see cref="ArgumentNullException"/> is thrown.
/// </remarks>
public sealed class SearchResultsFromContextMapper
    : IMapper<SearchPipelineContext, SearchResults<EstablishmentSearchResults, SearchFacets>>
{
    /// <summary>
    /// Maps the provided <see cref="SearchPipelineContext"/> into a
    /// <see cref="SearchResults{TResults, TFacets}"/> containing wrapped
    /// establishment results and facet results.
    /// </summary>
    /// <param name="context">The pipeline context containing search results and facets.</param>
    /// <returns>
    /// A populated <see cref="SearchResults{TResults, TFacets}"/> instance.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="context"/> or any required component is null.
    /// </exception>
    public SearchResults<EstablishmentSearchResults, SearchFacets> Map(SearchPipelineContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        EstablishmentSearchResult[] mapped =
            context.Get<EstablishmentSearchResult[]>()
            ?? throw new ArgumentNullException(nameof(context),
                "SearchPipelineContext does not contain EstablishmentSearchResult[].");

        List<SearchFacet> facets =
            context.Get<List<SearchFacet>>()
            ?? throw new ArgumentNullException(nameof(context),
                "SearchPipelineContext does not contain List<SearchFacet>.");

        EstablishmentSearchResults wrappedResults = new(mapped);
        SearchFacets wrappedFacets = new(facets);

        return new SearchResults<EstablishmentSearchResults, SearchFacets>
        {
            Results = wrappedResults,
            FacetResults = wrappedFacets
        };
    }
}
