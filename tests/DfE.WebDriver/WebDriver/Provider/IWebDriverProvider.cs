using OpenQA.Selenium;

namespace DfE.WebDriver.WebDriver.Provider;

internal interface IWebDriverProvider
{
    // TODO overloads
    IWebDriver GetDriver(WebDriverSessionContext spec);
}
