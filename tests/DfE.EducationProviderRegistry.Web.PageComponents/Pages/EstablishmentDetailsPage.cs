using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class EstablishmentDetailsPage
{
    private readonly ISearchContext _context;

    public EstablishmentDetailsPage(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public GovUkTable GetEstablishmentDetailsTable()
    {
        return
            _context
                .Find("#details .govuk-table")?
                .ToGovUkTable() ?? throw new ArgumentException("Could not find establishment details table");
    }
}
