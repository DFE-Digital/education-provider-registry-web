using DfE.Document.Abstractions;
using DfE.EducationProviderRegistry.Web.PageComponents;

namespace DfE.EducationProviderRegistry.Web.SharedTests.WebDriver.Search;

public sealed class SearchResultsComponent
{
    //private readonly WebDriverWait _defaultWaiter;
    private readonly ISearchContext _context;

    public SearchResultsComponent(ISearchContext context)
    {
        _context = context;
        // TODO wrap provider
        //_defaultWaiter = new(_driver, TimeSpan.FromSeconds(15));
        //_defaultWaiter.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
    }

    public IReadOnlyCollection<SearchResult> GetSearchResults()
    {

        //_defaultWaiter.Until((driver) => FindResults(driver).Count > 0);

        return
            [.. _context.FindAll(".search-results .govuk-table")
            .Select((tableElement) => tableElement.ToGovUkTable())
            .Select((table) => new SearchResult(
                Name: table.Caption ?? string.Empty,
                Type: table.Rows["Type"].Text))
            ];
    }

    //private static ReadOnlyCollection<IWebElement> FindResults(IWebDriver driver) => driver.FindElements(ResultTables);
}

public sealed record SearchResult(string Name, string Type);
