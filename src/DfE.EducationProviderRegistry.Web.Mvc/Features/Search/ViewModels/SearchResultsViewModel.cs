using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

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
    public bool HasResults => TotalSearchResultsCount >= 1;

    /// <summary>
    /// Property determining whether we have more than one search result.
    /// </summary>
    public bool HasMoreThanOneResult => TotalSearchResultsCount > 1;

    /// <summary>
    /// Property determining the number of search results.
    /// </summary>
    public int TotalSearchResultsCount => TotalEstablishmentResults;

    /// <summary>
    /// Determines whether there are filters in the results
    /// </summary>
    public bool HasFilters => Facets?.Count > 0;

    public SearchRequestViewModel SearchRequest { get; set; } = new();

    public int TotalEstablishmentResults { get; set; }

    public int TotalPages =>
    SearchRequest.RecordsPerPage <= 0
        ? 0
        : (int)Math.Ceiling(
            TotalEstablishmentResults /
            (double)SearchRequest.RecordsPerPage);

    public bool HasPreviousPage =>
        SearchRequest.PageNumber > 1;

    public bool HasNextPage =>
        SearchRequest.PageNumber < TotalPages;
}