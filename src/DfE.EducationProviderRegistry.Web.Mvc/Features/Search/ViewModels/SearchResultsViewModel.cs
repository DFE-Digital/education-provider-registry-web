using DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels.Filters;
using DfE.EducationProviderRegistry.Web.ViewComponents.Table;

namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class SearchResultsViewModel
{
    private List<IFilterViewModel> _filters = [];

    public required List<GovUkTable> SearchAggregateResults { get; set; }

    public List<IFilterViewModel> Filters
    {
        get => _filters;
        set => _filters = value ?? [];
    }

    public string? PrimarySearchTerms { get; set; }

    public string? SecondarySearchTerms { get; set; }

    public string? SelectedSortDirection { get; set; }

    public bool HasResults =>
        TotalSearchResults >= 1;

    public bool HasFilters =>
        Filters.Count > 0;

    public SearchRequestViewModel SearchRequest { get; set; } = new();

    public int TotalSearchResults { get; set; }

    public int TotalPages =>
        SearchRequest.RecordsPerPage <= 0
            ? 0
            : (int)Math.Ceiling(
                TotalSearchResults /
                (double)SearchRequest.RecordsPerPage);

    public bool HasPreviousPage =>
        SearchRequest.PageNumber > 1;

    public bool HasNextPage =>
        SearchRequest.PageNumber < TotalPages;
}
