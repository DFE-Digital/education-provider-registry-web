using DfE.Document.Abstractions;
using IElement = DfE.Document.Abstractions.IElement;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class BreadcrumbsComponent
{
    private readonly ISearchContext _context;

    public BreadcrumbsComponent(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public IReadOnlyList<Breadcrumb> Breadcrumbs()
    {
        IReadOnlyCollection<IElement> breadcrumbItems =
            _context.FindAll(
                "#breadcrumbs-nav .govuk-breadcrumbs__list-item a");

        return breadcrumbItems.Select((element) =>
            new Breadcrumb(
                Text: element.Text.Trim(),
                Link: element.Attribute("href") ?? string.Empty))
            .ToList();
    }
}

public sealed record Breadcrumb(string Text, string Link);
