using DfE.Document.Abstractions;
using DfE.Document.WebDriver;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.EducationProviderRegistry.Web.SharedTests.WebDriver;

public sealed class WebDriverQueryHandler : IQueryHandler
{
    private readonly IWebDriver _driver;

    public WebDriverQueryHandler(IWebDriver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);
        _driver = driver;
    }

    public TResult Handle<TResult>(Func<Document.Abstractions.ISearchContext, TResult> query)
    {
        ArgumentNullException.ThrowIfNull(query);

        WebDriverWait wait = new(
            _driver,
            TimeSpan.FromSeconds(15));

        wait.IgnoreExceptionTypes(
            typeof(StaleElementReferenceException));

        return wait.Until(driver =>
        {

            WebDriverSearchContext context = new(driver);
            return query(context);
        });
    }
}
