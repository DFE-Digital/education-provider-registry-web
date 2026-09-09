using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.Mvc.UITests.Search;

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
        IEnumerable<SearchResult> elements = _defaultWaiter.Until((driver) =>
        {
            return driver.FindElements(ResultRecords)
                .Select((result) => result.ToGovUkTable())
                .Select((table) => new SearchResult(Name: table.Caption ?? string.Empty, Type: table.Rows["Type"]));
        });

        return [.. elements];
    }
}

public sealed record SearchResult(string Name, string Type);