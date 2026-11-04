using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.UseCases.Response;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed class SearchResultsToViewModelMapper : IMapper<SearchResultsMappingContext, SearchResultsViewModel>
{
    private readonly IMapper<IReadOnlyCollection<SearchAggregateResult>, List<GovUkTable>> _searchAggregateResultsToViewModelMapper;
    private readonly IMapper<IReadOnlyCollection<SearchFacet>, List<FacetViewModel>> _facetResultsToFacetsViewModelMapper;

    public SearchResultsToViewModelMapper(
        IMapper<IReadOnlyCollection<SearchAggregateResult>, List<GovUkTable>> searchAggregateResultsToViewModelMapper,
        IMapper<IReadOnlyCollection<SearchFacet>, List<FacetViewModel>> facetResultsToFacetsViewModelMapper)
    {
        ArgumentNullException.ThrowIfNull(searchAggregateResultsToViewModelMapper);
        ArgumentNullException.ThrowIfNull(facetResultsToFacetsViewModelMapper);

        _searchAggregateResultsToViewModelMapper = searchAggregateResultsToViewModelMapper;
        _facetResultsToFacetsViewModelMapper = facetResultsToFacetsViewModelMapper;
    }

    public SearchResultsViewModel Map(
        SearchResultsMappingContext input)
    {
        ArgumentNullException.ThrowIfNull(input);

        SearchResponse searchResponse = input.SearchResponse.Model;

        List<FacetViewModel> facets =
            searchResponse.FacetedResults is not null
                ? _facetResultsToFacetsViewModelMapper.Map(
                    searchResponse.FacetedResults.Facets)
                : [];

        MarkSelectedFacetValues(
            facets,
            input.SearchRequest.SelectedFacets);

        return new SearchResultsViewModel
        {
            PrimarySearchTerms =
                input.SearchRequest.What!,

            SecondarySearchTerms =
                input.SearchRequest.Where,

            SearchRequest =
                input.SearchRequest,

            SelectedSortDirection = input.SearchRequest.Sort,

            SearchAggregateResults =
                searchResponse.SearchProviderResults is not null
                    ? _searchAggregateResultsToViewModelMapper.Map(
                        searchResponse
                            .SearchProviderResults
                            .SearchResultCollection)
                    : [],

            TotalSearchResults =
                searchResponse.TotalNumberOfResults,

            Facets = facets
        };
    }

    private static void MarkSelectedFacetValues(
        List<FacetViewModel> facets,
        Dictionary<string, List<string>>? selectedFacets)
    {
        if (selectedFacets is null)
        {
            return;
        }

        foreach (FacetViewModel facet in facets)
        {
            if (!selectedFacets.TryGetValue(
                    facet.Name,
                    out List<string>? selectedValues))
            {
                continue;
            }

            UpdateFacetSelections(
                facet,
                selectedValues);
        }
    }

    private static void UpdateFacetSelections(
        FacetViewModel facet,
        List<string> selectedValues)
    {
        for (int index = 0; index < facet.Values.Count; index++)
        {
            FacetValueViewModel value = facet.Values[index];

            facet.Values[index] = value with
            {
                IsSelected = selectedValues.Any(
                    selectedValue =>
                        !string.IsNullOrWhiteSpace(
                            selectedValue) &&
                        string.Equals(
                            selectedValue,
                            value.Value,
                            StringComparison.OrdinalIgnoreCase))
            };
        }
    }
}
