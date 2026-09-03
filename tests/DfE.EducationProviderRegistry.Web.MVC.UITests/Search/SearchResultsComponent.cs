using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Search;

internal sealed class SearchResultsComponent
{
    private readonly WebDriverWait _defaultWaiter;
    private readonly IWebDriver _driver;

    private static By ResultRecords => By.CssSelector(".search-results .govuk-table");

    public SearchResultsComponent(IWebDriver driver)
    {
        _driver = driver;
        _defaultWaiter = new(_driver, TimeSpan.FromSeconds(15));
        _defaultWaiter.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
    }

    public IReadOnlyCollection<SearchResult> GetSearchResults()
    {

        _defaultWaiter.Until((driver) => FindResults(driver).Count > 0);

        return [.. _defaultWaiter.Until((driver) =>
            FindResults(driver))
            .Select((result) => result.ToGovUkTable())
            .Select((table) => new SearchResult(
                Name: table.Caption ?? string.Empty,
                Type: table.Rows["Type"]))
            ];
    }

    private static ReadOnlyCollection<IWebElement> FindResults(IWebDriver driver) => driver.FindElements(ResultRecords);
}

public sealed record SearchResult(string Name, string Type);