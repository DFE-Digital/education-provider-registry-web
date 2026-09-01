using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Search;

internal sealed class SearchResults
{
    private readonly WebDriverWait _defaultWaiter;
    private readonly IWebDriver _driver;

    private static By ResultRecords => By.CssSelector(".search-results .govuk-table");

    public SearchResults(IWebDriver driver)
    {
        _driver = driver;
        _defaultWaiter = new(_driver, TimeSpan.FromSeconds(15));
        _defaultWaiter.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
    }

    public IReadOnlyCollection<SearchResult> GetSearchResults()
    {
        return
            _defaultWaiter.Until((driver) =>
            {
                ReadOnlyCollection<IWebElement> elements = driver.FindElements(ResultRecords);

                return elements
                    .Select((result) => result.ToGovUkTable())
                    .Select((table) => new SearchResult(
                        Name: table.Caption ?? string.Empty,
                        Type: table.Rows["Type"]))
                    .ToArray();
            });
    }
}

public static class WebElementExtensions
{
    public static GovUkTable ToGovUkTable(this IWebElement table)
    {
        string? caption = table
            .FindElements(By.CssSelector("caption"))
            .SingleOrDefault()?
            .Text
            .Trim();

        IReadOnlyDictionary<string, string> rows =
            table.FindElements(By.CssSelector("tbody tr"))
                 .ToDictionary(
                     (row) => row.FindElement(By.CssSelector("th")).Text.Trim(),
                     (row) => row.FindElement(By.CssSelector("td")).Text.Trim());

        return new GovUkTable
        {
            Caption = caption,
            Rows = rows
        };
    }
}

public sealed class GovUkTable
{
    public string? Caption { get; init; }

    public IReadOnlyDictionary<string, string> Rows { get; init; } = new Dictionary<string, string>();

    public string? this[string key]
        => Rows.TryGetValue(key, out string? value) ?
            value : null;

    public string? this[int key]
        => Rows.ElementAtOrDefault(key).Value;
}

public sealed record SearchResult(string Name, string Type);