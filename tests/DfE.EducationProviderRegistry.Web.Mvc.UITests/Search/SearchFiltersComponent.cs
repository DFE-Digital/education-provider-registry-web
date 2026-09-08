using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Search;

internal sealed class SearchFiltersComponent
{
    private static By FiltersDropdowns => By.CssSelector(".filter-section");
    private static By SubmitFilters => By.CssSelector(".filter-panel [type=submit]");
    private static By SelectedFilters => By.CssSelector(".app-selected-filters__list li");

    private readonly WebDriverWait _defaultWait;

    public SearchFiltersComponent(IWebDriver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);
        _defaultWait = new(driver, TimeSpan.FromSeconds(15));
    }

    public void FilterBy(string facetLabel, string facetValueLabel)
    {
        ExpandFacet(facetLabel);

        SelectFacetValue(facetLabel, facetValueLabel);

        _defaultWait.ClickOn(SubmitFilters);
    }


    public IReadOnlyCollection<SelectedFilter> GetSelectedFilters()
    {
        return [..
                _defaultWait
                    .FindMany(SelectedFilters)
                    .Select((element) =>
                        new SelectedFilter(element))
            ];
    }

    public string? GetFacetValueValue(string facetLabel, string targetFacetValueLabel)
    {
        By targetId = GetFacetValueLocator(_defaultWait, facetLabel, targetFacetValueLabel);

        return _defaultWait.Until(
            (wait) =>
                wait.FindElement(targetId).GetAttribute("id"));

    }

    private void ExpandFacet(string filterContainerLabel) =>
        _defaultWait
            .ClickOn((context) =>
                FindFacet(_defaultWait, filterContainerLabel));

    private void SelectFacetValue(string facetLabel, string facetValueLabel)
    {
        _defaultWait.ClickOn((context) =>
            context.FindElement(
                GetFacetValueLocator(_defaultWait, facetLabel, facetValueLabel)));
    }

    private static By GetFacetValueLocator(IWait<IWebDriver> context, string facetLabel, string targetFacetValueLabel)
    {
        IWebElement? targetFacet = FindFacet(context, facetLabel);

        DefaultWait<IWebElement> wait = new(targetFacet);

        // Facet value from facet that matches target label

        IWebElement? matchingLabel =
            wait
                .FindMany(By.CssSelector(".govuk-label"))
                // details behaviour when collapsed .Text behaves incorrectly, so we use GetAttribute("textContent") to get the correct label text
                .SingleOrDefault((label) => label.GetAttribute("textContent")?.Contains(targetFacetValueLabel, StringComparison.OrdinalIgnoreCase) ?? false);

        string id = matchingLabel?.GetAttribute("for") ?? throw new InvalidOperationException($"Could not find label with text {targetFacetValueLabel}");

        return By.Id(id);
    }

    private static IWebElement FindFacet(IWait<IWebDriver> waitContext, string label)
    {
        IWebElement facetContainer =
            waitContext.Until((context) =>
            {
                // Matches text
                return waitContext
                    .FindMany(FiltersDropdowns)
                    .Select((filter) => filter.FindElement(By.CssSelector(".govuk-details__summary-text")))
                    .SingleOrDefault((filter) => filter.Text.Contains(label, StringComparison.OrdinalIgnoreCase));
            });

        return facetContainer;

    }
}

public sealed record SelectedFilter
{
    public SelectedFilter(ISearchContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        IWebElement button = context.FindElement(By.CssSelector("button"));

        Name = button.GetAttribute("name")
            ?? throw new ArgumentException("Selected filter does not have a name attribute");

        Value = button.GetAttribute("value")
            ?? throw new ArgumentException("Selected filter does not have a value attribute");

        string[] parts = Value.Split('|');

        if (parts.Length != 2)
        {
            throw new ArgumentException($"Expected value in format '<FilterName>|<FilterId>' but received '{Value}'");
        }

        FilterName = parts[0];
        FilterId = parts[1];

        Text = button.Text.Trim();
    }

    public string Name { get; }

    public string Value { get; }

    public string FilterName { get; }

    public string FilterId { get; }

    public string Text { get; }
}