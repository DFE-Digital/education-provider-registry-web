using System.Text;
using DfE.Document.Abstractions;
using HttpMethod = System.Net.Http.HttpMethod;
using IElement = DfE.Document.Abstractions.IElement;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class SearchFiltersComponent
{
    private readonly ISearchContext _context;

    public SearchFiltersComponent(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public List<Filter> GetFilters()
    {
        List<Filter> filters = [];

        foreach (IElement filterElement in _context.FindAll(".filter-section"))
        {
            string filterName = filterElement.Find(".govuk-details__summary-text")?.Text?.Trim() ?? string.Empty;

            List<FilterValue> filterValues = [];

            foreach (IElement valueElement in filterElement.FindAll(".govuk-checkboxes__item"))
            {
                string label = valueElement.Find("label")?.Text?.Trim() ?? string.Empty;

                IElement input = valueElement.Find("input") ?? throw new InvalidOperationException("Input element not found");

                filterValues.Add(
                    new FilterValue(
                        Label: label,
                        Selected: input.Attribute("checked") is not null,
                        Value: input.Attribute("value") ?? string.Empty));
            }

            filters.Add(new Filter(filterName, filterValues));
        }
        return filters;
    }

    public List<Filter> GetFacetFilters()
    {
        return [.. GetFilters().Where(filter => filter.FilterValues.Count != 0)];
    }

    // TODO ensure it's embedded within FORM else submission won't submit it natively
    public Task<HttpResponseMessage> RemoveFilterAsync(
        HttpClient client,
        Uri currentUri,
        string facet,
        string facetValue,
        CancellationToken ct = default)
    {
        string removalButtonValue = $"{facet}|{facetValue}";

        IElement? element = _context.FindAll($"[value='{removalButtonValue}']").SingleOrDefault() ??
            throw new ArgumentException($"Element for facet {facet} and facetValue {facetValue} could not be found");

        HttpRequestMessage request =
            CreateUpdatedHttpRequest(
                baseUri: currentUri,
                addQueryParams: [new(element.Attribute("name")!, removalButtonValue)]
            );

        return client.SendAsync(request, ct);
    }

    public Task<HttpResponseMessage> ClearFiltersAsync(HttpClient client, Uri currentUri, CancellationToken ct)
    {
        string buttonName = "ClearFilters";
        string buttonLocator = $"[name='{buttonName}']";

        IElement element = _context.FindAll(buttonLocator).SingleOrDefault() ??
            throw new ArgumentException($"Could not find button locator {buttonLocator} to clear filters");

        HttpRequestMessage request =
            CreateUpdatedHttpRequest(
                baseUri: currentUri,
                addQueryParams: [new(buttonName!, element.Attribute("value")!)]
            );

        return client.SendAsync(request, ct);
    }

    private static HttpRequestMessage CreateUpdatedHttpRequest(
        Uri baseUri,
        IReadOnlyList<KeyValuePair<string, string>> addQueryParams)
    {
        StringBuilder outputQueryParams = addQueryParams.Aggregate(
            seed: new StringBuilder(),
            func: (sb, kv) => sb.Append($"&{kv.Key}={kv.Value}"));

        return new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new(string.Concat(baseUri, outputQueryParams.ToString()))
        };
    }
}

public sealed record Filter(string Name, List<FilterValue> FilterValues);
public sealed record FilterValue(string Label, bool Selected, string Value);
