using DfE.Core.Libraries.CrossCutting.Mapper;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Establishment;
using DfE.EducationProviderRegistry.Core.Query.Search.Application.Models.Search;
using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;
using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.Mappers;

public sealed class SearchResultsToViewModelMapper :
    IMapper<SearchResultsMappingContext, SearchResultsViewModel>
{
    private readonly IMapper<
        IReadOnlyCollection<EstablishmentSearchResult>,
        List<GovUkTable>>
        _establishmentSearchResultsToViewModelMapper;

    private readonly IMapper<
        IReadOnlyCollection<SearchFacet>,
        List<FacetViewModel>>
        _facetResultsToFacetsViewModelMapper;

    public SearchResultsToViewModelMapper(
        IMapper<
            IReadOnlyCollection<EstablishmentSearchResult>,
            List<GovUkTable>>
            establishmentSearchResultsToViewModelMapper,
        IMapper<
            IReadOnlyCollection<SearchFacet>,
            List<FacetViewModel>>
            facetResultsToFacetsViewModelMapper)
    {
        ArgumentNullException.ThrowIfNull(
            establishmentSearchResultsToViewModelMapper);

        ArgumentNullException.ThrowIfNull(
            facetResultsToFacetsViewModelMapper);

        _establishmentSearchResultsToViewModelMapper =
            establishmentSearchResultsToViewModelMapper;

        _facetResultsToFacetsViewModelMapper =
            facetResultsToFacetsViewModelMapper;
    }

    public SearchResultsViewModel Map(
        SearchResultsMappingContext input)
    {
        ArgumentNullException.ThrowIfNull(input);

        var searchResponse = input.SearchResponse.Model
            ?? throw new ArgumentException(
                "SearchResponse model cannot be null.",
                nameof(input));

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
                input.SearchRequest.SearchKeywords!,

            SecondarySearchTerms =
                input.SearchRequest.Address,

            SearchRequest =
                input.SearchRequest,

            EstablishmentResults =
                searchResponse.EstablishmentResults is not null
                    ? _establishmentSearchResultsToViewModelMapper.Map(
                        searchResponse
                            .EstablishmentResults
                            .EstablishmentCollection)
                    : [],

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
                            value.Key,
                            StringComparison.OrdinalIgnoreCase))
            };
        }
    }
}