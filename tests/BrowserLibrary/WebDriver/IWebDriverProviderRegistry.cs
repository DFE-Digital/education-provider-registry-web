namespace BrowserLibrary.WebDriver;

internal interface IWebDriverProviderRegistry
{
    IWebDriverProvider GetProvider(string key);
}
