using Docker.DotNet.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Search;

internal sealed class SearchFilters
{
    private static By FiltersDropdowns => By.CssSelector(".filter-section");
    private static By SubmitFilters => By.CssSelector(".filter-panel [type=submit]");
    private static By SelectedFilters => By.CssSelector(".app-selected-filters__list li");

    private readonly WebDriverWait _defaultWait;
    private readonly IWebDriver _driver;

    public SearchFilters(IWebDriver driver)
    {
        _defaultWait = new(driver, TimeSpan.FromSeconds(15));
        _driver = driver;
    }

    public void FilterBy(string facetLabel, string facetValueLabel)
    {
        ExpandFacet(facetLabel);

        _defaultWait.Until(
            (driver) => driver.FindElement(
                By.Id(
                    GetFacetValueId(
                        driver,
                        facetLabel,
                        facetValueLabel)))
                .Click());

        _defaultWait.Until((driver) => driver.FindElement(SubmitFilters).Click());
    }

    public IReadOnlyCollection<SelectedFilter> GetSelectedFilters()
    {
        return
        [
            .. _defaultWait.Until(
                driver => driver.FindElements(SelectedFilters))
            .Select(element => new SelectedFilter(element))
        ];
    }

    public string? GetFacetValueValue(string facetLabel, string targetFacetValueLabel)
    {
        string targetId = GetFacetValueId(_driver, facetLabel, targetFacetValueLabel);
        return _defaultWait.Until((driver) => driver.FindElement(By.Id(targetId))).GetAttribute("id");
    }

    private void ExpandFacet(string filterContainerLabel) =>
        _defaultWait.Until(
            (driver) =>
                FindFilterDropdown(driver, filterContainerLabel)
                .Click());

    private static string GetFacetValueId(IWebDriver driver, string facetLabel, string targetFacetValueLabel)
    {
        IWebElement? targetFacet =
            FindFilterDropdown(driver, facetLabel)
                .FindElements(By.CssSelector(".govuk-label"))
                // details behaviour when collapsed .Text behaves incorrectly, so we use GetAttribute("textContent") to get the correct label text
                .SingleOrDefault((label) =>
                    label.GetAttribute("textContent")?.Contains(targetFacetValueLabel, StringComparison.OrdinalIgnoreCase) ?? false);

        return targetFacet?.GetAttribute("for") ?? throw new InvalidOperationException($"Could not find label with text {targetFacetValueLabel}");
    }

    private static IWebElement FindFilterDropdown(IWebDriver driver, string label)
    {
        IWebElement? container =
            driver
                .FindElements(FiltersDropdowns)
                .SingleOrDefault((filter) =>
                    filter.FindElement(By.CssSelector(".govuk-details__summary-text")).Text
                        .Contains(label, StringComparison.OrdinalIgnoreCase));

        return container is null ?
            throw new ArgumentException($"Could not find filter container with label {label}")
                : container;
    }

    public sealed record SelectedFilter
    {
        public SelectedFilter(IWebElement element)
        {
            ArgumentNullException.ThrowIfNull(element);

            IWebElement button =
                element.FindElement(By.CssSelector("button"));

            Name = button.GetAttribute("name")
                ?? throw new ArgumentException(
                    "Selected filter does not have a name attribute");

            Value = button.GetAttribute("value")
                ?? throw new ArgumentException(
                    "Selected filter does not have a value attribute");

            string[] parts = Value.Split('|');

            if (parts.Length != 2)
            {
                throw new ArgumentException(
                    $"Expected value in format '<FilterName>|<FilterId>' but received '{Value}'");
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
}