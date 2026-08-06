using OpenQA.Selenium;

namespace DfE.WebDriver.WebDriver.Options;

internal interface IWebDriverOptionsFactory<out TOptions>
    where TOptions : DriverOptions
{
    TOptions CreateOptions(WebDriverSessionContext context);
}