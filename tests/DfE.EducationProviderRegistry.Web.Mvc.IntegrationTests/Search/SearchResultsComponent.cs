namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

public sealed class SearchResultsComponent
{
    private readonly IHtmlDocument _document;

    public SearchResultsComponent(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }

    public IReadOnlyList<SearchResult> GetSearchResults()
    {
        return _document.QuerySelectorAll(".search-results .govuk-table")
            .Select((element) =>
                new SearchResult(
                    Name: element.QuerySelector(".govuk-table__caption")?.Text() ?? string.Empty))
            .ToList();
    }
}

public sealed record SearchResult(string Name);
