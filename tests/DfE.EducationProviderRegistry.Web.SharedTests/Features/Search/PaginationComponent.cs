using AngleSharp.Html.Dom;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

public sealed class PaginationComponent
{
    private readonly IHtmlDocument _document;

    public PaginationComponent(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }

    public bool Displayed()
    {
        return _document.QuerySelector(".govuk-pagination") is not null;
    }

    public bool DisplaysPreviousLink()
    {
        return _document.QuerySelector(".govuk-pagination__prev") is not null;
    }

    public bool DisplaysNextLink()
    {
        return _document.QuerySelector(".govuk-pagination__next") is not null;
    }

    public string GetCurrentPage()
    {
        return _document.QuerySelector(".govuk-pagination__item--current")?.TextContent.Trim() ??
            throw new ArgumentException("Could not locate current page");
    }
}