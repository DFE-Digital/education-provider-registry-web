using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents.Commands;
using DfE.EducationProviderRegistry.Web.PageComponents.GovUk;

namespace DfE.EducationProviderRegistry.Web.PageComponents.Pages;

public sealed class TemporaryUISearchFiltersComponent
{
    private const string FilterSectionsSelector = ".filter-section";
    private const string FilterLabelSelector = ".govuk-label";
    private const string SelectedFiltersSelector = ".app-selected-filters__list li";
    private const string SelectedFilterButtonSelector = "button";
    private const string SubmitFiltersSelector = ".filter-panel [type=submit]";

    private readonly GovUkDetailsComponent _details;
    private readonly IQueryHandler _queryHandler;
    private readonly ICommandDispatcher _dispatcher;

    public TemporaryUISearchFiltersComponent(
        IQueryHandler queryHandler,
        ICommandDispatcher dispatcher)
    {
        ArgumentNullException.ThrowIfNull(queryHandler);
        ArgumentNullException.ThrowIfNull(dispatcher);

        _queryHandler = queryHandler;
        _dispatcher = dispatcher;
        _details = new GovUkDetailsComponent(dispatcher);
    }

    public void FilterBy(
        string facetLabel,
        string facetValueLabel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(facetLabel);
        ArgumentException.ThrowIfNullOrWhiteSpace(facetValueLabel);

        _details.Expand(facetLabel);

        string targetId = GetFacetValueId(facetLabel, facetValueLabel);

        _dispatcher.Dispatch<ClickCommand, ClickCommandModel>(
            new ClickCommandModel
            {
                Selector = $"#{targetId}"
            });

        _dispatcher.Dispatch<ClickCommand, ClickCommandModel>(
            new ClickCommandModel
            {
                Selector = SubmitFiltersSelector
            });
    }

    public IReadOnlyCollection<SelectedFilter> GetSelectedFilters()
    {
        return
        [
            .. _queryHandler.Handle((context) =>
                    context.FindAll(SelectedFiltersSelector)
                    .Select(CreateSelectedFilter))
        ];
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
                    $"Could not find filter container with label {facetLabel}");

            IElement? targetFacetValue =
                targetFacet
                    .FindAll(FilterLabelSelector)
                    .SingleOrDefault(label =>
                        label.Text.Contains(
                            targetFacetValueLabel,
                            StringComparison.OrdinalIgnoreCase));

            return targetFacetValue?.Attribute("for")
                ?? throw new InvalidOperationException(
                    $"Could not find label with text {targetFacetValueLabel}");
        });
    }

    private static SelectedFilter CreateSelectedFilter(IElement selectedFilter)
    {
        IElement button =
            selectedFilter.Find(SelectedFilterButtonSelector) ??
                throw new InvalidOperationException("Could not find the selected-filter button.");

        string value =
            button.Attribute("value") ??
                throw new InvalidOperationException("Selected filter does not have a value attribute.");

        return new SelectedFilter(value)
        {
            Text = button.Text.Trim(),
            Name = button.Attribute("name")
        };
    }
}

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
                $"but received '{value}'",
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
