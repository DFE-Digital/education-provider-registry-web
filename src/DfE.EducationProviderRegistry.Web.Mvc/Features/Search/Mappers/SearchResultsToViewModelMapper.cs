using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Filters;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed class SearchResultsToViewModelMapper : IMapper<SearchResultsMappingContext, SearchResultsViewModel>
{
    private readonly IMapper<
        IReadOnlyCollection<SearchAggregateResult>,
        List<GovUkTable>> _searchAggregateResultsToViewModelMapper;

    private readonly IMapper<
        FacetResultsMappingContext,
        IReadOnlyCollection<IFilterViewModel>> _facetResultsToFiltersViewModelMapper;

    private readonly IMapper<
        AvailableFiltersMappingContext,
        IReadOnlyCollection<IFilterViewModel>> _filtersToFiltersViewModelMapper;

    public SearchResultsToViewModelMapper(
        IMapper<
            IReadOnlyCollection<SearchAggregateResult>,
            List<GovUkTable>> searchAggregateResultsToViewModelMapper,
        IMapper<
            FacetResultsMappingContext,
            IReadOnlyCollection<IFilterViewModel>> facetResultsToFiltersViewModelMapper,
        IMapper<
            AvailableFiltersMappingContext,
            IReadOnlyCollection<IFilterViewModel>> filtersToFiltersViewModelMapper)
    {
        ArgumentNullException.ThrowIfNull(searchAggregateResultsToViewModelMapper);
        ArgumentNullException.ThrowIfNull(facetResultsToFiltersViewModelMapper);
        ArgumentNullException.ThrowIfNull(filtersToFiltersViewModelMapper);

        _searchAggregateResultsToViewModelMapper = searchAggregateResultsToViewModelMapper;
        _facetResultsToFiltersViewModelMapper = facetResultsToFiltersViewModelMapper;
        _filtersToFiltersViewModelMapper = filtersToFiltersViewModelMapper;
    }

    public SearchResultsViewModel Map(
        SearchResultsMappingContext input)
    {
        ArgumentNullException.ThrowIfNull(input);

        SearchResponse searchResponse =
            input.SearchResponse.Model;


        return new SearchResultsViewModel
        {
            PrimarySearchTerms =
                input.SearchRequest.What,

            SecondarySearchTerms =
                input.SearchRequest.Where,

            SearchRequest =
                input.SearchRequest,

            SelectedSortDirection =
                input.SearchRequest.Sort,

            SearchAggregateResults =
                searchResponse.SearchProviderResults is not null
                    ? _searchAggregateResultsToViewModelMapper.Map(
                        searchResponse
                            .SearchProviderResults
                            .SearchResultCollection)
                    : [],

            TotalSearchResults =
                searchResponse.TotalNumberOfResults,

            Filters =
                MapFilters(
                    request: input.SearchRequest,
                    input.SearchResponse.Model.FacetedResults?.Facets ?? [],
                    input.SearchResponse.Model.AvailableFilters)
        };
    }

    private List<IFilterViewModel> MapFilters(
        SearchRequestViewModel request,
        IReadOnlyCollection<SearchFacet> facets,
        IReadOnlyCollection<string> filters)
    {
        return [
            // facets
            .._facetResultsToFiltersViewModelMapper.Map(
                    new FacetResultsMappingContext(facets ?? [], request.SelectedFacets)),
            // filters
            .._filtersToFiltersViewModelMapper.Map(
                new AvailableFiltersMappingContext(filters, request))
            ];
    }
}
