using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class SearchResultsComponent
{
    private readonly ISearchContext _context;
    private readonly TextComponent _textComponent;

    public SearchResultsComponent(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
        _textComponent = new();
    }

    public string GetHeading()
    {
        return _context.Find("h1")?.Text.Trim() ?? string.Empty;
    }

    public string GetTotalResultsLabel()
    {
        const string locator = ".results-header .govuk-body";

        return _context.Find(locator)?.Text.Trim() ??
            throw new ArgumentException($"Could not find results header with locator: {locator}");
    }

    public IReadOnlyList<SearchResult> GetSearchResults()
    {
        return [.. _context.FindAll(".search-results .govuk-table")
        .Select((element) =>
            new SearchResult(
                Name: _textComponent.GetContent(
                    element.Find(".govuk-table__caption")!),
                Values:
                    element.FindAll("tbody tr")
                        .Select((row) => (
                            _textComponent.GetContent(row.Find("th")!),
                            _textComponent.GetContent(row.Find("td")!)
                        ))
                        .ToArray()
            )
        )
        ];
    }
}

public sealed record class SearchResult(TextContent Name, IEnumerable<(TextContent key, TextContent value)> Values);

