using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System.Text;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

internal sealed class SearchFiltersComponent
{
    private readonly IHtmlDocument _document;

    public SearchFiltersComponent(IHtmlDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        _document = document;
    }

    public List<Filter> GetFilters()
    {
        List<Filter> filters = [];

        foreach (IElement filterElement in _document.QuerySelectorAll(".filter-section"))
        {
            string filterName = filterElement.QuerySelector(".govuk-details__summary-text")?.TextContent?.Trim() ?? string.Empty;

            List<FilterValue> filterValues = [];
            
            foreach (IElement valueElement in filterElement.QuerySelectorAll(".govuk-checkboxes__item"))
            {
                string label = valueElement.QuerySelector("label")?.TextContent?.Trim() ?? string.Empty;
                
                IElement input = valueElement.QuerySelector("input") ?? throw new InvalidOperationException("Input element not found");
                
                filterValues.Add(
                    new FilterValue(
                        Label: label, 
                        Selected: input.HasAttribute("checked"),
                        Value: input.GetAttribute("value") ?? string.Empty));
            }

            filters.Add(new Filter(filterName, filterValues));
        }
        return filters;
    }

    // TODO ensure it's embedded within FORM else submission won't submit it natively
    public Task<HttpResponseMessage> RemoveFilterAsync(
        HttpClient client, 
        string facetLabel, 
        string facetValue, 
        CancellationToken ct = default)
    {
        string removalButtonValue = $"{facetLabel}|{facetValue}";

        IElement? element = _document.QuerySelectorAll($"[value='{removalButtonValue}']").SingleOrDefault() ??
            throw new ArgumentException($"Element for facetLabel {facetLabel} and facetValue {facetValue} could not be found");

        HttpRequestMessage request = 
            CreateUpdatedHttpRequest(
                _document,
                addQueryParams: [ new(element.GetAttribute("name")!, removalButtonValue) ]
            );

        return client.SendAsync(request, ct);
    }

    public Task<HttpResponseMessage> ClearFiltersAsync(HttpClient client, CancellationToken ct)
    {
        string buttonName = "ClearFilters";
        string buttonLocator = $"[name='{buttonName}']";

        IElement element = _document.QuerySelectorAll(buttonLocator).SingleOrDefault() ??
            throw new ArgumentException($"Could not find button locator {buttonLocator} to clear filters");

        HttpRequestMessage request =
            CreateUpdatedHttpRequest(
                _document,
                addQueryParams: [new(buttonName!, element.GetAttribute("value")!)]
            );

        return client.SendAsync(request, ct);
    }

    private static HttpRequestMessage CreateUpdatedHttpRequest(
        IHtmlDocument document, 
        IReadOnlyList<KeyValuePair<string, string>> addQueryParams)
    {
        StringBuilder outputQueryParams = addQueryParams.Aggregate(
            seed: new StringBuilder(),
            func: (sb, kv) => sb.Append($"&{kv.Key}={kv.Value}"));

        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new(string.Concat(document.BaseUri, outputQueryParams.ToString()))
        };
    }
}

public sealed record Filter(string Name, List<FilterValue> FilterValues);
public sealed record FilterValue(string Label, bool Selected, string Value);