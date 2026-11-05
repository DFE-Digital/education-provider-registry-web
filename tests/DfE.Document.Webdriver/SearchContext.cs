using ISearchContext = DfE.EducationProviderRegistry.Web.SharedTests.Infrastructure.Document.Abstractions.ISearchContext;

namespace DfE.Document.Webdriver;

public static class SearchContext
{
    public static ISearchContext From(OpenQA.Selenium.ISearchContext driver)
    {
        ArgumentNullException.ThrowIfNull(driver);
        return new WebDriverSearchContext(driver);
    }
}
