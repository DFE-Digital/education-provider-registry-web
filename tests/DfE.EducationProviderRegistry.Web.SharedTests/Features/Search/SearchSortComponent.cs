using AngleSharp.Html.Dom;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

public sealed class SearchSortComponent
{
    private readonly IHtmlDocument _document;

    public SearchSortComponent(
        IHtmlDocument document)
    {
        _document = document;
    }

    public string? GetSelectedSortDirection()
    {
        return _document
            .QuerySelector("#sort option[selected]")
            ?.GetAttribute("value");
    }
}