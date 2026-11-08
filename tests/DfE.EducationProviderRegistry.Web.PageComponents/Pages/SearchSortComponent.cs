using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class SearchSortComponent
{
    private readonly ISearchContext _context;

    public SearchSortComponent(ISearchContext context)
    {
        _context = context;
    }

    public string? SelectedSortDirection()
    {
        return _context
            .Find("#sort option[selected]")?
            .Attribute("value");
    }
}
