namespace DfE.EducationProviderRegistry.Web.Mvc.Search.ViewModels;

public sealed class SearchRequestViewModel
{
    private Dictionary<string, List<string>>? _selectedFacets;

    /// <summary>
    /// The dictionary of selected facets (grouped by facet name key) provisioned
    /// by binding to user input.
    /// </summary>
    public Dictionary<string, List<string>>? SelectedFacets
    {
        get
        {
            if (ClearFilters) _selectedFacets = null;
            return _selectedFacets;
        }
        set { _selectedFacets = value; }
    }

    /// <summary>
    /// The search keyword provisioned by binding to user input.
    /// </summary>
    public string? SearchKeywords { get; set; }

    /// <summary>
    /// The number of records per page (derived indirectly from appsettings)
    /// </summary>
    public int RecordsPerPage { get; set; }

    /// <summary>
    /// The current page
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Indicates that the 'Clear filters' button was pressed
    /// </summary>
    public bool ClearFilters { get; set; }

    /// <summary>
    /// The offset is used to skip the defined number of records retrieved
    /// on a search request. We set page number -1 to ensure the first page
    /// is not skipped, etc (so we'll have (1-1) * 20 = 0, which ensures no
    /// values are ignored in the first instance.
    /// </summary>
    public int Offset => (PageNumber - 1) * RecordsPerPage;

}