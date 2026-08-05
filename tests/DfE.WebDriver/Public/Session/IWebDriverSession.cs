using OpenQA.Selenium;

namespace DfE.WebDriver.Public.Session;

public interface IWebDriverSession
{
    ValueTask<IWebDriver> StartDriverAsync(CancellationToken cancellationToken = default);
}