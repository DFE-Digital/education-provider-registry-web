namespace DfE.EducationProviderRegistry.Web.Mvc.Features.Search.ViewModels;

public sealed class SearchRequestViewModel
{
    private Dictionary<string, List<string>>? _selectedFacets;

    /// <summary>
    /// Gets or sets the dictionary of selected facet values, grouped by facet name.
    /// </summary>
    /// <remarks>
    /// This property is populated via model binding from the search UI, where each
    /// facet name maps to a list of selected values.
    ///
    /// If <see cref="ClearFilters"/> is <c>true</c>, the getter resets the backing
    /// field to <c>null</c> and returns <c>null</c>, ensuring that all filters are
    /// cleared for the request.
    /// </remarks>
    public Dictionary<string, List<string>>? SelectedFacets
    {
        get
        {
            if (ClearFilters)
                _selectedFacets = null;

            return _selectedFacets;
        }
        set { _selectedFacets = value; }
    }

    /// <summary>
    /// Gets or sets the search keywords entered by the user.
    /// </summary>
    /// <remarks>
    /// This value is typically used to perform text‑based search across establishments.
    /// </remarks>
    public string? SearchKeywords { get; set; }

    /// <summary>
    /// Gets or sets the number of records to display per page.
    /// </summary>
    /// <remarks>
    /// This value is usually configured indirectly via application settings and
    /// determines how many results are shown in each page of the search results.
    /// </remarks>
    public int RecordsPerPage { get; set; }

    /// <summary>
    /// Gets or sets the current page number for the search results.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1</c>.  
    /// Used together with <see cref="RecordsPerPage"/> to calculate the
    /// <see cref="Offset"/> for pagination.
    /// </remarks>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating whether the user pressed the
    /// “Clear filters” button.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, the <see cref="SelectedFacets"/> getter returns <c>null</c>,
    /// ensuring that no facet filters are applied to the search request.
    /// </remarks>
    public bool ClearFilters { get; set; }

    /// <summary>
    /// Gets the number of records to skip when retrieving paged search results.
    /// </summary>
    /// <remarks>
    /// Calculated as:
    /// <code>
    /// (PageNumber - 1) * RecordsPerPage
    /// </code>
    /// 
    /// For example:
    /// <list type="bullet">
    /// <item><description>Page 1 → offset = 0</description></item>
    /// <item><description>Page 2 → offset = RecordsPerPage</description></item>
    /// <item><description>Page 3 → offset = 2 × RecordsPerPage</description></item>
    /// </list>
    /// </remarks>
    public int Offset => (PageNumber - 1) * RecordsPerPage;
}
