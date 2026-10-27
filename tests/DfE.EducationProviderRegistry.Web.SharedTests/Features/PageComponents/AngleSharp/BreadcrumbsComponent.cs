using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.AngleSharp;

public sealed class BreadcrumbsComponent
{
    private readonly IHtmlDocument _document;

    public BreadcrumbsComponent(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }

    public IReadOnlyList<Breadcrumb> Breadcrumbs()
    {
        IHtmlCollection<IElement> breadcrumbItems =
            _document.QuerySelectorAll(
                "#breadcrumbs-nav .govuk-breadcrumbs__list-item a");

        return breadcrumbItems.Select((element) =>
            new Breadcrumb(
                Text: element.TextContent.Trim(),
                Link: element.GetAttribute("href") ?? string.Empty))
            .ToList();
    }
}

public sealed record Breadcrumb(string Text, string Link);
