using System.Text;
using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.Commands;
using DfE.EducationProviderRegistry.Web.PageComponents.GovUk;
using HttpMethod = System.Net.Http.HttpMethod;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class SearchFiltersComponent
{
    private const string FilterSectionsSelector = ".filter-section";
    private const string FilterValueLabelSelector = ".govuk-label";
    private const string SelectedFiltersSelector = ".app-selected-filters__list li";
    private const string SelectedFilterButtonSelector = "button";
    private const string SubmitFiltersSelector = ".filter-panel [type=submit]";

    private readonly IQueryHandler _queryHandler;
    private readonly ICommandHandler? _commandHandler;

    public SearchFiltersComponent(
        IQueryHandler queryHandler,
        ICommandHandler? commandHandler = null)
    {
        ArgumentNullException.ThrowIfNull(queryHandler);

        _queryHandler = queryHandler;
        _commandHandler = commandHandler;
    }

    public List<Filter> GetFilters()
    {
        return _queryHandler.Handle(context =>
        {
            List<Filter> filters = [];

            foreach (IElement filterElement in context.FindAll(FilterSectionsSelector))
            {
                string filterName =
                    filterElement
                        .Find(".govuk-details__summary-text")?
                        .Text
                        .Trim()
                    ?? string.Empty;

                List<FilterValue> filterValues = [];

                foreach (IElement valueElement in
                    filterElement.FindAll(".govuk-checkboxes__item"))
                {
                    string label =
                        valueElement
                            .Find("label")?
                            .Text
                            .Trim()
                        ?? string.Empty;

                    IElement input =
                        valueElement.Find("input")
                        ?? throw new InvalidOperationException(
                            "Input element not found.");

                    filterValues.Add(
                        new FilterValue(
                            Label: label,
                            Selected: input.Attribute("checked") is not null,
                            Value: input.Attribute("value") ?? string.Empty));
                }

                filters.Add(
                    new Filter(
                        filterName,
                        filterValues));
            }

            return filters;
        });
    }

    public List<Filter> GetFacetFilters()
    {
        return
        [
            .. GetFilters()
                .Where(filter => filter.FilterValues.Count != 0)
        ];
    }

    public void FilterBy(
        string facetLabel,
        string facetValueLabel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(facetLabel);
        ArgumentException.ThrowIfNullOrWhiteSpace(facetValueLabel);

        if(_commandHandler is null)
        {
            throw new InvalidOperationException("No command handler passed for command FilterBy");
        }
        GovUkDetailsComponent details = new(_commandHandler);
        details.Expand(facetLabel);

        string targetId = GetFacetValueId(
            facetLabel,
            facetValueLabel);

        _commandHandler.Handle<ClickCommand, ClickCommandModel>(
            new ClickCommandModel
            {
                Selector = $"#{targetId}"
            });

        _commandHandler.Handle<ClickCommand, ClickCommandModel>(
            new ClickCommandModel
            {
                Selector = SubmitFiltersSelector
            });

        // Validate Filter applied state transition

        _queryHandler.Handle(context =>
            context
                .FindAll(SelectedFiltersSelector)
                .Select(CreateSelectedFilter)
                .Any(filter =>
                    filter.Text.Contains(
                        facetValueLabel,
                        StringComparison.OrdinalIgnoreCase)));
    }

    public IReadOnlyCollection<SelectedFilter> GetSelectedFilters()
    {
        return _queryHandler.Handle<IReadOnlyCollection<SelectedFilter>>(context =>
        {
            return
            [
                .. context
                    .FindAll(SelectedFiltersSelector)
                    .Select(CreateSelectedFilter)
            ];
        });
    }

    // TODO ensure it is embedded within FORM,
    // otherwise submission will not submit it natively.
    public Task<HttpResponseMessage> RemoveFilterAsync(
        HttpClient client,
        Uri currentUri,
        string facet,
        string facetValue,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(currentUri);
        ArgumentException.ThrowIfNullOrWhiteSpace(facet);
        ArgumentException.ThrowIfNullOrWhiteSpace(facetValue);

        string removalButtonValue = $"{facet}|{facetValue}";

        HttpRequestMessage request =
            _queryHandler.Handle(context =>
            {
                IElement element =
                    context
                        .FindAll($"[value='{removalButtonValue}']")
                        .SingleOrDefault()
                    ?? throw new ArgumentException(
                        $"Element for facet {facet} and facetValue {facetValue} could not be found.");

                string elementName =
                    element.Attribute("name")
                    ?? throw new InvalidOperationException(
                        $"Element for facet {facet} and facetValue {facetValue} does not have a name.");

                return CreateUpdatedHttpRequest(
                    baseUri: currentUri,
                    addQueryParams:
                    [
                        new(
                            elementName,
                            removalButtonValue)
                    ]);
            });

        return client.SendAsync(request, ct);
    }

    public Task<HttpResponseMessage> ClearFiltersAsync(
        HttpClient client,
        Uri currentUri,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(currentUri);

        const string buttonName = "ClearFilters";
        const string buttonLocator = "[name='ClearFilters']";

        HttpRequestMessage request =
            _queryHandler.Handle(context =>
            {
                IElement element =
                    context
                        .FindAll(buttonLocator)
                        .SingleOrDefault()
                    ?? throw new ArgumentException(
                        $"Could not find button locator {buttonLocator} to clear filters.");

                string buttonValue =
                    element.Attribute("value")
                    ?? throw new InvalidOperationException(
                        $"Button located by {buttonLocator} does not have a value.");

                return CreateUpdatedHttpRequest(
                    baseUri: currentUri,
                    addQueryParams:
                    [
                        new(
                            buttonName,
                            buttonValue)
                    ]);
            });

        return client.SendAsync(request, ct);
    }

    private string GetFacetValueId(
        string facetLabel,
        string targetFacetValueLabel)
    {
        return _queryHandler.Handle(context =>
        {
            IElement targetFacet =
                context
                    .FindAll(FilterSectionsSelector)
                    .SingleOrDefault(filter =>
                        filter
                            .Find(".govuk-details__summary-text")?
                            .Text
                            .Contains(
                                facetLabel,
                                StringComparison.OrdinalIgnoreCase)
                        ?? false)
                ?? throw new ArgumentException(
                    $"Could not find filter container with label {facetLabel}.");

            IElement targetFacetValue =
                targetFacet
                    .FindAll(FilterValueLabelSelector)
                    .SingleOrDefault(label =>
                        label.Text.Contains(
                            targetFacetValueLabel,
                            StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException(
                    $"Could not find label with text {targetFacetValueLabel}.");

            return targetFacetValue.Attribute("for")
                ?? throw new InvalidOperationException(
                    $"Label with text {targetFacetValueLabel} does not have a for attribute.");
        });
    }

    private static SelectedFilter CreateSelectedFilter(
        IElement selectedFilter)
    {
        IElement button =
            selectedFilter.Find(SelectedFilterButtonSelector)
            ?? throw new InvalidOperationException(
                "Could not find the selected-filter button.");

        string value =
            button.Attribute("value")
            ?? throw new InvalidOperationException(
                "Selected filter does not have a value attribute.");

        return new SelectedFilter(value)
        {
            Text = button.Text.Trim(),
            Name = button.Attribute("name")
        };
    }

    private static HttpRequestMessage CreateUpdatedHttpRequest(
        Uri baseUri,
        IReadOnlyList<KeyValuePair<string, string>> addQueryParams)
    {
        StringBuilder outputQueryParams =
            addQueryParams.Aggregate(
                seed: new StringBuilder(),
                func: (builder, queryParam) =>
                    builder.Append(
                        $"&{queryParam.Key}={queryParam.Value}"));

        return new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(
                string.Concat(
                    baseUri,
                    outputQueryParams.ToString()))
        };
    }
}

public sealed record Filter(
    string Name,
    List<FilterValue> FilterValues);

public sealed record FilterValue(
    string Label,
    bool Selected,
    string Value);

public sealed record class SelectedFilter
{
    public SelectedFilter(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        string[] parts = value.Split('|');

        if (parts.Length != 2)
        {
            throw new ArgumentException(
                $"Expected value in format '<FilterName>|<FilterId>' " +
                $"but received '{value}'.",
                nameof(value));
        }

        FilterName = parts[0];
        FilterId = parts[1];
    }

    public required string? Name { get; init; }

    public required string Text { get; init; }

    public string FilterId { get; }

    public string FilterName { get; }
}
