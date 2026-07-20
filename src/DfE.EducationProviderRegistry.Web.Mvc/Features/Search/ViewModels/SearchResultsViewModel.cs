using DfE.EducationProviderRegistry.Web.Mvc.ViewComponents;
using DfE.EducationProviderRegistry.Web.Mvc.ViewModels;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class SearchResultsViewModel
{
    public required List<GovUkTable> EstablishmentResults { get; set; }

    private List<FacetViewModel>? _facets;

    /// <summary>
    /// View model representation of the available facets.
    /// </summary>
    public List<FacetViewModel>? Facets
    {
        get => _facets;
        set => _facets = value?.OrderBy(facet => facet.Name).ToList();
    }

    public string? PrimarySearchTerms { get; set; }

    public string? SecondarySearchTerms { get; set; }

    /// <summary>
    /// Property determining whether we have at least one search result.
    /// </summary>
    public bool HasResults => SearchResultsCount >= 1;

    /// <summary>
    /// Property determining whether we have more than one search result.
    /// </summary>
    public bool HasMoreThanOneResult => SearchResultsCount > 1;

    /// <summary>
    /// Property determining the number of search results.
    /// </summary>
    public int SearchResultsCount => EstablishmentResults?.Count ?? 0;

    /// <summary>
    /// Determines whether there are filters in the results
    /// </summary>
    public bool HasFilters => Facets?.Count > 0;

    public IReadOnlyCollection<SelectedFilterViewModel> SelectedFilters { get; set; } = [];

    public SearchRequestViewModel SearchRequest { get; set; } = new();
}