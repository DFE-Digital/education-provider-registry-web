using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.MVC.UITests.Search;

internal sealed class SearchPanel
{
    private readonly WebDriverWait _defaultWaiter;

    private static By IdentityInput => By.CssSelector("#SearchKeywords");
    private static By LocationInput => By.CssSelector("#Address");

    private static By SortDropdown => By.CssSelector("#sort");
    private static By SubmitSearch => By.CssSelector(".dfe-search-panel__actions [type=submit]");

    public SearchPanel(IWebDriver driver)
    {
        _defaultWaiter = new(driver, TimeSpan.FromSeconds(15));
    }

    public void Search((string identityTerm, string locationTerm) terms)
    {
        EnterTerm(_defaultWaiter, terms.identityTerm, IdentityInput);
        EnterTerm(_defaultWaiter, terms.locationTerm, LocationInput);
        Submit(_defaultWaiter);
    }

    private static void Submit(WebDriverWait driver)
    {
        driver.Until(
            (driver) =>
                driver
                    .FindElement(SubmitSearch)
                    .Submit());
    }

    private static void EnterTerm(WebDriverWait driver, string term, By locator)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return;
        }

        driver.Until((driver) =>
            driver
                .FindElement(locator)
                .SendKeys(term)
        );
    }

    public void SortBy(string _, SortDirection descending)
    {
        _defaultWaiter.Until((driver) =>
        {
            SelectElement sortDropdown = new(driver.FindElement(SortDropdown));

            string sortValue = descending switch
            {
                SortDirection.Descending => "za",
                _ => "az"
            };

            sortDropdown.SelectByValue(sortValue);
        });
    }

    public enum SortDirection
    {
        Ascending,
        Descending
    }
}