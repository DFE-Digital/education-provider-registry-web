using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Search;

public sealed class SearchResults
{
    private readonly WebDriverWait _defaultWaiter;
    private readonly IWebDriver _driver;

    private static By ResultRecords => By.CssSelector(".search-results .govuk-table");
    private static By ResultName => By.CssSelector(".govuk-table__caption");

    public SearchResults(IWebDriver driver)
    {
        _driver = driver;
        _defaultWaiter = new(_driver, TimeSpan.FromSeconds(15));
    }

    public IReadOnlyCollection<SearchResult> GetSearchResults()
    {
        IReadOnlyCollection<IWebElement>? results = _defaultWaiter.Until(driver =>
        {
            var elements = driver.FindElements(ResultRecords);
            return elements.Count > 0 ? elements : null;
        });

        return [
            .. results?.Select(
                (result) =>
                {
                    string name = result.FindElement(ResultName).Text;
                    return new SearchResult(name);
                }) ?? []];
    }
}

public sealed record SearchResult(string Name);