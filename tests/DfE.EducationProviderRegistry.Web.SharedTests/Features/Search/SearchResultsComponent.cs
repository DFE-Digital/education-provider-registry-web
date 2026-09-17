using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.Search;

public sealed class SearchResultsComponent
{
    private readonly IHtmlDocument _document;

    public SearchResultsComponent(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }

    public string GetHeading()
    {
        return _document.QuerySelector("h1")?.Text().Trim() ?? string.Empty;
    }

    public string GetTotalResults()
    {
        const string locator = ".results-header .govuk-body";

        return _document.QuerySelector(locator)?.Text().Trim() ??
            throw new ArgumentException($"Could not find results header with locator: {locator}");
    }

    public IReadOnlyList<SearchResult> GetSearchResults()
    {
        return _document.QuerySelectorAll(".search-results .govuk-table")
            .Select((element) =>
                new SearchResult(
                    Name: element.QuerySelector(".govuk-table__caption")?
                            .Text()
                            .ReplaceLineEndings()
                            .Trim() ?? throw new ArgumentException("Search result did not have caption")))
            .ToList();
    }
}

public sealed record SearchResult(string Name);
