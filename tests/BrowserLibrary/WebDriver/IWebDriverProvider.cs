using OpenQA.Selenium;

namespace BrowserLibrary.WebDriver;

internal interface IWebDriverProvider
{
    // TODO overloads
    IWebDriver GetDriver(WebDriverSessionRequest spec);
}
