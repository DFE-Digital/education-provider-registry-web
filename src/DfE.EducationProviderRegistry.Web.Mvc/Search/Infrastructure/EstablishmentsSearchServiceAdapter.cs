using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Core.Providers;
using DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure.Pipeline;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Search.Infrastructure;

public sealed class EstablishmentsSearchServiceAdapter
    : ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>
{
    private readonly IQueryable<Establishment> _rootQuery;
    private readonly IIdSearchProvider<Establishment> _idProvider;
    private readonly IReadOnlyList<ISearchPipelineStep> _pipeline;
    private readonly IMapper<
        SearchPipelineContext, SearchResults<
        EstablishmentSearchResults, SearchFacets>> _searchResultsFromContextMapper;
    private readonly IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>> _searchRequestFiltersToCoreFiltersMapper;

    public EstablishmentsSearchServiceAdapter(
        IQueryable<Establishment> rootQuery,
        IIdSearchProvider<Establishment> idProvider,
        IFacetProvider<Establishment> facetProvider,
        IEnumerable<ISearchPipelineStep> pipeline,
        IMapper<Establishment, EstablishmentSearchResult> establishmentToSearchResultMapper,
        IMapper<SearchPipelineContext, SearchResults<EstablishmentSearchResults, SearchFacets>> searchResultsFromContextMapper,
        IMapper<ReadOnlyCollection<FilterRequest>, ReadOnlyCollection<SearchFilterRequest>> searchRequestFiltersToCoreFiltersMapper)
    {
        _rootQuery = rootQuery ??
            throw new ArgumentNullException(nameof(rootQuery));
        _idProvider = idProvider ??
            throw new ArgumentNullException(nameof(idProvider));
        _searchResultsFromContextMapper = searchResultsFromContextMapper ??
            throw new ArgumentNullException(nameof(searchResultsFromContextMapper));
        _searchRequestFiltersToCoreFiltersMapper = searchRequestFiltersToCoreFiltersMapper ??
            throw new ArgumentNullException(nameof(searchRequestFiltersToCoreFiltersMapper));

        ArgumentNullException.ThrowIfNull(pipeline);
        _pipeline = pipeline.ToList().AsReadOnly();

        ArgumentNullException.ThrowIfNull(facetProvider);
        ArgumentNullException.ThrowIfNull(establishmentToSearchResultMapper);        
    }

    public async Task<SearchResults<EstablishmentSearchResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IReadOnlyList<string> ids =
            await _idProvider.GetMatchingIdsAsync(
                searchTerm: request.SearchKeyword,
                pageSize: 50,
                offset: request.Offset,
                searchFilterRequests: _searchRequestFiltersToCoreFiltersMapper
                    .Map(request.SearchFilterRequests.AsReadOnly()),
                cancellationToken);

        if (ids.Count == 0)
        {
            return new SearchResults<EstablishmentSearchResults, SearchFacets>();
        }

        List<Establishment> establishments =
            await _rootQuery.Where(establishment =>
                ids.Contains(establishment.Urn)).ToListAsync(cancellationToken);

        SearchPipelineContext context = new();
        context.Set(ids);
        context.Set(establishments);
        context.Set(request.Facets.ToList());

        foreach (ISearchPipelineStep step in _pipeline)
        {
            step.Execute(context, cancellationToken);
        }

        return _searchResultsFromContextMapper.Map(context);
    }
}