using AngleSharp.Dom;
using AngleSharp.Html.Dom;

namespace DfE.EducationProviderRegistry.Web.Mvc.IntegrationTests.Search;

internal sealed class SearchFilters
{
    private readonly IHtmlDocument _document;

    public SearchFilters(IHtmlDocument document)
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
}

public sealed record Filter(string Name, List<FilterValue> FilterValues);
public sealed record FilterValue(string Label, bool Selected, string Value);