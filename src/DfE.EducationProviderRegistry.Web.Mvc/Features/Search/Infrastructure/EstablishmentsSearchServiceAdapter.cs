using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Infrastructure;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Filter;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Data.DatabaseModels.Models;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Filtering;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Core.Providers;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure.Pipeline;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Infrastructure;

/// <summary>
/// Provides a high‑level orchestration layer for executing establishment search queries.
/// This adapter coordinates ID lookup, filter translation, pipeline execution, and final
/// result mapping into <see cref="SearchResults{TResults, TFacets}"/>.
/// </summary>
/// <remarks>
/// The adapter delegates search execution to an <see cref="ISearchProvider{T}"/>,
/// applies filter transformations, runs all configured pipeline steps, and produces
/// a fully populated search result including facets and mapped establishment projections.
/// </remarks>
public sealed class EstablishmentsSearchServiceAdapter
    : ISearchServiceAdapter<EstablishmentSearchResults, SearchFacets>
{
    /// <summary>
    /// Provider responsible for retrieving matching establishments based on search terms
    /// and filter criteria. This is typically backed by trigram search or other search
    /// orchestration mechanisms.
    /// </summary>
    private readonly ISearchProvider<Establishment> _idProvider;

    /// <summary>
    /// Ordered collection of pipeline steps that operate on the <see cref="SearchPipelineContext"/>
    /// to enrich, transform, or augment search results (e.g., ordering, facet resolution).
    /// </summary>
    private readonly IReadOnlyList<ISearchPipelineStep> _pipeline;

    /// <summary>
    /// Mapper responsible for converting the final <see cref="SearchPipelineContext"/> into
    /// a <see cref="SearchResults{TResults, TFacets}"/> instance containing establishment
    /// projections and facet values.
    /// </summary>
    private readonly IMapper<
        SearchPipelineContext, SearchResults<
        EstablishmentSearchResults, SearchFacets>> _searchResultsFromContextMapper;

    /// <summary>
    /// Mapper that converts MVC‑level <see cref="FilterRequest"/> objects into core
    /// <see cref="SearchFilterRequest"/> objects understood by the search provider.
    /// </summary>
    private readonly IMapper<
        ReadOnlyCollection<FilterRequest>,
        ReadOnlyCollection<SearchFilterRequest>> _searchRequestFiltersToCoreFiltersMapper;

    /// <summary>
    /// Creates a new instance of <see cref="EstablishmentsSearchServiceAdapter"/>.
    /// </summary>
    /// <param name="idProvider">Provider used to retrieve matching establishment IDs.</param>
    /// <param name="facetProvider">Facet provider required by the pipeline (validated only).</param>
    /// <param name="pipeline">Pipeline steps executed in order to enrich search results.</param>
    /// <param name="searchResultsFromContextMapper">Mapper converting pipeline context to final results.</param>
    /// <param name="searchRequestFiltersToCoreFiltersMapper">Mapper converting UI filter requests to core filters.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any required dependency is <c>null</c>.
    /// </exception>
    public EstablishmentsSearchServiceAdapter(
        ISearchProvider<Establishment> idProvider,
        IFacetProvider facetProvider,
        IEnumerable<ISearchPipelineStep> pipeline,
        IMapper<
            SearchPipelineContext,
            SearchResults<EstablishmentSearchResults, SearchFacets>> searchResultsFromContextMapper,
        IMapper<
            ReadOnlyCollection<FilterRequest>,
            ReadOnlyCollection<SearchFilterRequest>> searchRequestFiltersToCoreFiltersMapper)
    {
        _idProvider = idProvider ??
            throw new ArgumentNullException(nameof(idProvider));
        _searchResultsFromContextMapper = searchResultsFromContextMapper ??
            throw new ArgumentNullException(nameof(searchResultsFromContextMapper));
        _searchRequestFiltersToCoreFiltersMapper = searchRequestFiltersToCoreFiltersMapper ??
            throw new ArgumentNullException(nameof(searchRequestFiltersToCoreFiltersMapper));

        ArgumentNullException.ThrowIfNull(pipeline);
        _pipeline = pipeline.ToList().AsReadOnly();

        ArgumentNullException.ThrowIfNull(facetProvider);
    }

    /// <summary>
    /// Executes an establishment search using the provided request parameters. This includes:
    /// ID lookup, filter translation, pipeline execution, facet resolution, and final result mapping.
    /// </summary>
    /// <param name="request">Search request containing keyword, filters, and paging information.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="SearchResults{TResults, TFacets}"/> instance containing establishment results
    /// and computed facet values. If no establishments match, an empty result set is returned.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="request"/> is <c>null</c>.
    /// </exception>
    public async Task<SearchResults<EstablishmentSearchResults, SearchFacets>> SearchAsync(
        SearchServiceAdapterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        IReadOnlyList<Establishment> establishments =
            await _idProvider.GetMatchingIdsAsync(
                searchTerm: request.SearchKeyword,
                pageSize: 50,
                offset: request.Offset,
                searchFilterRequests: _searchRequestFiltersToCoreFiltersMapper
                    .Map(request.SearchFilterRequests.AsReadOnly()),
                cancellationToken);

        ReadOnlyCollection<string?> availableEstablishmentIids =
            establishments.Select(establishment =>
                establishment.Urn).ToList().AsReadOnly();

        if (availableEstablishmentIids.Count == 0)
        {
            return new SearchResults<EstablishmentSearchResults, SearchFacets>();
        }

        SearchPipelineContext context = new();
        context.Set(availableEstablishmentIids);
        context.Set(establishments);
        context.Set(new List<string> { "EstablishmentTypeId" });

        foreach (ISearchPipelineStep step in _pipeline)
        {
            step.Execute(context, cancellationToken);
        }

        return _searchResultsFromContextMapper.Map(context);
    }
}
