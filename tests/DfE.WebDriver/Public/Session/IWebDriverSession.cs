using OpenQA.Selenium;

namespace BrowserLibrary.Public.Session;

public interface IWebDriverSession
{
    ValueTask<IWebDriver> StartDriverAsync(CancellationToken cancellationToken = default);
}