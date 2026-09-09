using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.Mvc.UITests.Search;

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
        return
        [
            .._defaultWait
                .FindMany(SelectedFilters)
                .Select(element => new SelectedFilter(element))
        ];
    }

    public string? GetFacetValueValue(
        string facetLabel,
        string targetFacetValueLabel)
    {
        IWebElement facet = FindFacet(facetLabel);

        IWebElement label =
            FindFacetValueLabel(
                facet,
                targetFacetValueLabel);

        return label.GetAttribute("for");
    }

    private void ExpandFacet(string facetLabel)
    {
        _defaultWait.ClickOn(_ => FindFacet(facetLabel));
    }

    private void SelectFacetValue(
        string facetLabel,
        string facetValueLabel)
    {
        _defaultWait.ClickOn((driver) =>
        {
            IWebElement facet = FindFacet(facetLabel);

            return
                FindFacetValueLabel(
                    facet,
                    facetValueLabel);
        });
    }

    private IWebElement FindFacet(string label)
    {
        return 
            _defaultWait.Until(driver =>
                driver
                    .FindElements(FiltersDropdowns)
                    .SingleOrDefault(filter =>
                        filter.FindElements(By.CssSelector(".govuk-details__summary-text"))
                            .Any((element) =>  
                                element.Text.Contains(label, StringComparison.OrdinalIgnoreCase))));
    }

    private static IWebElement FindFacetValueLabel(
        IWebElement facet,
        string valueLabel)
    {
        return facet
            .FindElements(By.CssSelector(".govuk-label"))
            .SingleOrDefault((facetValueLabelElement) =>
                facetValueLabelElement
                    .GetAttribute("textContent")?
                    .Contains(valueLabel, StringComparison.OrdinalIgnoreCase) ?? false) ?? 
                            throw new InvalidOperationException($"Could not find label with text '{valueLabel}'");
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