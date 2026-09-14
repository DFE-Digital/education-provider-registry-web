using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class SearchResultsViewModel
{
    public required List<GovUkTable> ProviderResults { get; set; }

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

    public string? SelectedSortDirection { get; set; }

    public bool HasResults => TotalEstablishmentResults >= 1;
    public bool HasMoreThanOneResult => TotalEstablishmentResults > 1;
    public bool HasFilters => Facets?.Count > 0;

    public SearchRequestViewModel SearchRequest { get; set; } = new();

    public int TotalEstablishmentResults { get; set; }

    public int TotalPages =>
        SearchRequest.RecordsPerPage <= 0
            ? 0
            : (int)Math.Ceiling(
                TotalEstablishmentResults / (double)SearchRequest.RecordsPerPage);

    public bool HasPreviousPage =>
        SearchRequest.PageNumber > 1;

    public bool HasNextPage =>
        SearchRequest.PageNumber < TotalPages;
}