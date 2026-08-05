using OpenQA.Selenium;

namespace DfE.WebDriver.WebDriver;

internal interface IWebDriverProvider
{
    // TODO overloads
    IWebDriver GetDriver(WebDriverSessionRequest spec);
}
