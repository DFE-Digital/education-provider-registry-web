using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents;
using DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.AngleSharp;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.Search.Components;

public sealed class SearchResultsComponent
{
    private readonly IHtmlDocument _document;
    private readonly TextComponent _textComponent;

    public SearchResultsComponent(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
        _textComponent = new();
    }

    public string GetHeading()
    {
        return _document.QuerySelector("h1")?.Text().Trim() ?? string.Empty;
    }

    public string GetTotalResultsLabel()
    {
        const string locator = ".results-header .govuk-body";

        return _document.QuerySelector(locator)?.Text().Trim() ??
            throw new ArgumentException($"Could not find results header with locator: {locator}");
    }

    public IReadOnlyList<SearchResult> GetSearchResults()
    {
        return [.. _document.QuerySelectorAll(".search-results .govuk-table")
            .Select((element) => //TODO map to GDS table and then map over to SearchResult
                new SearchResult(
                    Name: _textComponent.GetContent(element.QuerySelector(".govuk-table__caption")!),
                    Values:
                        element.QuerySelectorAll("tbody tr").ToDictionary(
                            (row) => _textComponent.GetContent(row.QuerySelector("th")!),
                            (row) => _textComponent.GetContent(row.QuerySelector("td")!))
                )
            )
        ];
    }
}

public sealed record class SearchResult(TextContent Name, IReadOnlyDictionary<TextContent, TextContent> Values);

