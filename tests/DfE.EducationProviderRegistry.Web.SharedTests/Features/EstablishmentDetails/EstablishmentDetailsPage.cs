using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.AngleSharp;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.WebDriver;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.EstablishmentDetails;

public sealed class EstablishmentDetailsPage
{
    private readonly IHtmlDocument _document;

    public EstablishmentDetailsPage(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }

    public GovUkTable GetEstablishmentDetailsTable()
    {
        return _document
            .QuerySelectorAll("#details .govuk-table")
            .SingleOrDefault()?
            .ToGovUkTable() ??
                throw new ArgumentException("Could not find establishment details table");
    }
}
