using DfE.Document.Abstractions;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class SearchResultsComponent
{
    private readonly IQueryHandler _queryHandler;
    private readonly TextComponent _textComponent;

    public SearchResultsComponent(IQueryHandler queryHandler)
    {
        ArgumentNullException.ThrowIfNull(queryHandler);
        _queryHandler = queryHandler;
        _textComponent = new();
    }

    public string GetHeading()
    {
        return _queryHandler.Handle(context => context.Find("h1")?.Text.Trim() ?? string.Empty);
    }

    public string GetTotalResultsLabel()
    {
        const string locator = ".results-header .govuk-body";

        return _queryHandler.Handle(context => context.Find(locator)?.Text.Trim() ??
            throw new ArgumentException($"Could not find results header with locator: {locator}"));
    }

    public IReadOnlyList<SearchResult> GetSearchResults()
    {
        return
            _queryHandler.Handle((context) =>
                context
                    .FindAll(".search-results .govuk-table"))
                    .Select((element) =>
                        new SearchResult(
                            Name: _textComponent.GetContent(
                                element.Find(".govuk-table__caption")!),
                            Values:
                                element.FindAll("tbody tr")
                                    .Select((row) => (
                                        _textComponent.GetContent(row.Find("th")!),
                                        _textComponent.GetContent(row.Find("td")!)))
                                    .ToList()))
                    .ToList();
    }
}

public sealed record class SearchResult(TextContent Name, IReadOnlyList<(TextContent key, TextContent value)> Values);

