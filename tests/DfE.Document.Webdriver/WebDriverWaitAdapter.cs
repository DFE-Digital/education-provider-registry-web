using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DfE.Document.Webdriver;

public sealed class WebDriverWaitAdapter
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

    private readonly IWebDriver _driver;

    public WebDriverWaitAdapter(IWebDriver driver)
    {
        ArgumentNullException.ThrowIfNull(driver);

        _driver = driver;
    }

    public TResult Until<TResult>(Func<IWebDriver, TResult> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        WebDriverWait wait = new(_driver, Timeout);

        wait.IgnoreExceptionTypes(
            typeof(StaleElementReferenceException));

        return wait.Until(action);
    }
}
