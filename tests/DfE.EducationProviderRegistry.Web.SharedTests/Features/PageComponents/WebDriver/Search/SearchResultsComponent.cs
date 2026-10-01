using System.Collections.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.SharedTests.Features.PageComponents.WebDriver.Search;

public sealed class SearchResultsComponent
{
    private readonly WebDriverWait _defaultWaiter;
    private readonly IWebDriver _driver;

    private static By ResultTables => By.CssSelector(".search-results .govuk-table");

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
            FindResults(driver)
            .Select((result) => result.ToGovUkTable())
            .Select((table) => new SearchResult(
                Name: table.Caption ?? string.Empty,
                Type: table.Rows["Type"].Text)))
            ];
    }

    private static ReadOnlyCollection<IWebElement> FindResults(IWebDriver driver) => driver.FindElements(ResultTables);
}

public sealed record SearchResult(string Name, string Type);
