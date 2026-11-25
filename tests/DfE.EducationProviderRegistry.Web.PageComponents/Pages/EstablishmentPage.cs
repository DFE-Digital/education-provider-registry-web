using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class EstablishmentPage
{
    private readonly ISearchContext _context;

    public EstablishmentPage(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public GovUkTable Details()
    {
        return
            _context
                .Find("#details .govuk-table")?
                .ToGovUkTable() ?? throw new ArgumentException("Could not find establishment details table");
    }
}
